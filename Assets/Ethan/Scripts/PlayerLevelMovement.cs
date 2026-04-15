using System.Collections;       
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLevelMovement : MonoBehaviour
{
    //Variables
    #region
    // Input Action Variables
    [SerializeField] InputActionReference leftRight;
    [SerializeField] InputActionReference jump;

    // Jump Variables
    public float jumpForce = 7.0f; // This is the force of the jump
    private bool jumpPressed; // This is a boolean that is used to check if the jump button is pressed
    public float inputThreshold = 0.5f; // This is the threshold for the leftRightInput
    public float groundCheckDistance = 1.5f; // This is the distance to check for the ground
    public float groundCheckStartHeight = .5f; // This is the height to start checking for the ground
    public LayerMask groundLayers; // This is the layer mask for the ground

    public Rigidbody playerRigidbody; // This is the rigidbody component of the player
    private Collider playerCollider; // Cached for Physics.IgnoreCollision with walls
    private Collider activeWallCollider; // Wall we're currently touching (set on enter, used when jumping off)

    // Lane Variables
    private Vector2 leftRightInput; // This is a vector2 that is used to store the value of the leftRightInput
    public int currentLane = 1; // This is the current lane of the player
    public float lefLaneX = -5.0f;
    public float centerLaneX = 0.0f;
    public float rightLaneX = 5.0f;
    private bool canReadLaneInput = true;
    public float laneChangeSpeed = 20.0f;// Lane Change Speed

    // Wall Run Variables
    private bool isWallRunning = false; // This is a boolean that is used to check if the player is wall running
    [HideInInspector] public Vector2 rightWallPosition;
    [HideInInspector] public Vector2 leftWallPosition;
    public float wallRunDelay = 0.2f; // This is the delay for the wall run
    public WallType wallType;
    public enum WallType
    {
        none,
        leftWall,
        rightWall
    }

    // Area type enum
    public enum AreaType // The type of area for wall running, close wall running, and normal movement
    {
        normal,
        wallRunning,
        closeWallRunning
    }
    public AreaType areaType = AreaType.normal;
    #endregion

    void Awake(){
        playerCollider = GetComponent<Collider>();
        groundLayers = LayerMask.GetMask("Ground"); // Get the layer mask for the ground
        if(playerRigidbody == null){
            playerRigidbody = GetComponent<Rigidbody>();
        }
        leftRight.action.performed += LeftRight;
    }
    

    public void LeftRight(InputAction.CallbackContext value){ // This is a function that is called when the leftRightInput is pressed
        leftRightInput = value.ReadValue<Vector2>(); // Get the value of the leftRightInput
        float xInput = leftRightInput.x; // Get the x input
        if (isWallRunning)
        { // If the player is wall running, then return
            if (areaType == AreaType.closeWallRunning)
            {

            }
            return;
        }
        if (Mathf.Abs(xInput) < inputThreshold){ // If the x input is less than the input threshold, then the player can read the lane input
            canReadLaneInput = true;
            return;
        }
        if(!canReadLaneInput){ // If the player cannot read the lane input, then return
            return;
        }
        switch (areaType)
        {
            default:
            case AreaType.normal:
                if(xInput > 0f){ // If the x input is greater than 0, then the player can move to the right lane
                    currentLane++;
                }
                else if(xInput < 0f){ // If the x input is less than 0, then the player can move to the left lane
                    currentLane--;
                }
                currentLane = Mathf.Clamp(currentLane, 0, 2); // Clamp the current lane between 0 and 2
                break;
            case AreaType.wallRunning:
                if (xInput > 0f)
                {
                    if (currentLane == 2)
                    {
                        isWallRunning = true;
                        playerRigidbody.useGravity = false;
                        wallType = WallType.rightWall;
                    }
                    currentLane++;
                }
                else if (xInput < 0f)
                {
                    if (currentLane == 0)
                    {
                        isWallRunning = true;
                        playerRigidbody.useGravity = false;
                        wallType = WallType.leftWall;
                    }
                    currentLane--;
                }
                currentLane = Mathf.Clamp(currentLane, -1, 3); // Clamp the current lane between 0 and 2
                break;
            case AreaType.closeWallRunning:
                if (xInput > 0f)
                {
                    if (currentLane == 1)
                    {
                        isWallRunning = true;
                        playerRigidbody.useGravity = false;
                        wallType = WallType.rightWall;
                    }
                    currentLane++;
                }
                else if (xInput < 0f)
                {
                    if (currentLane == 1)
                    {
                        isWallRunning = true;
                        playerRigidbody.useGravity = false;
                        wallType = WallType.leftWall;
                    }
                    currentLane--;
                }
                currentLane = Mathf.Clamp(currentLane, 0, 2);
                break;
        }
        canReadLaneInput = false; // Set the canReadLaneInput to false
    }
    public void Jump(InputValue value){ // This is a function that is called when the jump button is pressed argument is the value of the input
        if(value.isPressed){ // If the jump button is pressed, then set the jumpPressed to true
            jumpPressed = true;// Set the jumpPressed to true
        }
    }
    void FixedUpdate(){// This is a function that is called every fixed delta time
        if (isWallRunning && jumpPressed)
        {
            StopWallRun();
            jumpPressed = false;
        }
        switch (areaType)
        {
            default:
            case AreaType.normal:
                NormalMove();
                break;
            case AreaType.wallRunning:
                WallRunMove();
                break;
            case AreaType.closeWallRunning:
                CloseWallRunMove();
                break;
        }

        // Jump
        if(jumpPressed && IsGrounded()){
            Vector3 jumpVelocity = playerRigidbody.linearVelocity;// Get the velocity of the player by getting the velocity of the player's rigidbody
            jumpVelocity.y = 0f; // Set the y velocity of the player to 0
            playerRigidbody.linearVelocity = jumpVelocity; // Set the velocity of the player to the jump velocity
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Add a force to the player to make them jump by using the AddForce function which uses the arguments force, force mode, force being the force to apply to the player, and the force mode being the mode to apply the force in impulse mode which is a force that is applied instantly
            jumpPressed = false; // Set the jumpPressed to false
        }
    }
    
    void NormalMove()
    {
        float targetX = GetTargetLaneX(); // Get the target x position

        float newX = Mathf.MoveTowards(playerRigidbody.position.x, targetX, laneChangeSpeed * Time.fixedDeltaTime); // Move the player to the target x position by using the MoveTowards function which uses the arguments current position, target position, and the speed
        float xVelocity = (newX - playerRigidbody.position.x) / Time.fixedDeltaTime; // Calculate the x velocity by the difference between the new x position and the current x position divided by the fixed delta time which is the time it takes to complete one frame

        Vector3 velocity = playerRigidbody.linearVelocity; // Get the velocity of the player by getting the velocity of the player's rigidbody
        velocity.x = xVelocity; // Set the x velocity of the player to the x velocity
        playerRigidbody.linearVelocity = velocity; // Set the velocity of the player to the velocity
    }

    void WallRunMove()
    {
        float targetX = GetTargetLaneX(); // Get the target x position

        float newX = Mathf.MoveTowards(playerRigidbody.position.x, targetX, laneChangeSpeed * Time.fixedDeltaTime); // Move the player to the target x position by using the MoveTowards function which uses the arguments current position, target position, and the speed
        float xVelocity = (newX - playerRigidbody.position.x) / Time.fixedDeltaTime; // Calculate the x velocity by the difference between the new x position and the current x position divided by the fixed delta time which is the time it takes to complete one frame
        
        Vector3 velocity = playerRigidbody.linearVelocity; // Get the velocity of the player by getting the velocity of the player's rigidbody

        if (currentLane == 3 || currentLane == -1)
        {
            float targetY = GetTartgetLaneY();

            float newY = Mathf.MoveTowards(playerRigidbody.position.y, targetY, laneChangeSpeed * Time.fixedDeltaTime);
            float yVelocity = (newY - playerRigidbody.position.y) / Time.fixedDeltaTime;
            velocity.y = yVelocity;
            Debug.Log("Doing this");
        }

        velocity.x = xVelocity; // Set the x velocity of the player to the x velocity
        playerRigidbody.linearVelocity = velocity; // Set the velocity of the player to the velocity
    }

    void CloseWallRunMove()
    {
        float targetX = GetTargetLaneX(); // Get the target x position

        float newX = Mathf.MoveTowards(playerRigidbody.position.x, targetX, laneChangeSpeed * Time.fixedDeltaTime); // Move the player to the target x position by using the MoveTowards function which uses the arguments current position, target position, and the speed
        float xVelocity = (newX - playerRigidbody.position.x) / Time.fixedDeltaTime; // Calculate the x velocity by the difference between the new x position and the current x position divided by the fixed delta time which is the time it takes to complete one frame

        Vector3 velocity = playerRigidbody.linearVelocity; // Get the velocity of the player by getting the velocity of the player's rigidbody

        if (currentLane == 0 || currentLane == 2)
        {
            float targetY = GetTartgetLaneY();

            float newY = Mathf.MoveTowards(playerRigidbody.position.y, targetY, laneChangeSpeed * Time.fixedDeltaTime);
            float yVelocity = (newY - playerRigidbody.position.y) / Time.fixedDeltaTime;
            velocity.y = yVelocity;
            Debug.Log("Doing this");
        }

        velocity.x = xVelocity; // Set the x velocity of the player to the x velocity
        playerRigidbody.linearVelocity = velocity; // Set the velocity of the player to the velocity
    }

    float GetTargetLaneX(){ // This is a function that is called to get the target x position of the player
        if (areaType != AreaType.closeWallRunning)
        {
            if (currentLane == 0)
            { // If the current lane is 0, then return the left lane x position
                return lefLaneX;
            }
            if (currentLane == 1)
            { // If the current lane is 1, then return the center lane x position
                return centerLaneX;
            }
            if (currentLane == -1)
            {
                return leftWallPosition.x;
            }
            if (currentLane == 3)
            {
                return rightWallPosition.x;
            }
            return rightLaneX; // If the current lane is 2, then return the right lane x position
        }
        else
        {
            if (currentLane == 0)
            {
                return leftWallPosition.x;
            }
            if (currentLane == 2)
            {
                return rightWallPosition.x;
            }
            return centerLaneX;
        }
    }

    float GetTartgetLaneY()
    {
        switch (wallType)
        {
            default:
            case WallType.leftWall:
                return leftWallPosition.y;
            case WallType.rightWall:
                return rightWallPosition.y;
        }
    }

    public bool IsGrounded(){ // This is a function that is called to check if the player is grounded
        Vector3 rayStart = transform.position + Vector3.up * groundCheckStartHeight; // Get the start of the ray by adding the up vector to the position of the player and the ground check start height
        return Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayers); // Cast a ray down from the start of the ray to the ground check distance and check if the ray hits the ground layers
    }
    void StopWallRun(){
        isWallRunning = false;
        playerRigidbody.useGravity = true;
        if (areaType == AreaType.wallRunning)
        {
            switch (wallType)
            {
                case WallType.leftWall:
                    currentLane = 0;
                    break;
                case WallType.rightWall:
                    currentLane = 2;
                    break;
            }
        }
        else
        {
            currentLane = 1;
        }
        wallType = WallType.none;
    }

    void ReenableWallCollisionAfterDelay(Collider wall){ // This is a coroutine that is called to reenable the wall collision after a delay
        if (playerCollider != null && wall != null)
            Physics.IgnoreCollision(playerCollider, wall, false);
    }
}