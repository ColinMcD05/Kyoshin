using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;
public class PlayerControllerLevel : MonoBehaviour
{
    [SerializeField] private Rewind rewind;
    GameManager gameManager;
    #region Lane Variables | This is the x position of the left lane, center lane, and right lane
    public float lefLaneX = -5.0f;
    public float centerLaneX = 0.0f;
    public float rightLaneX = 5.0f;
    #endregion
    public float forwardSpeed = 8.0f;
    [HideInInspector] public float maxSpeed;
    [HideInInspector] public float minSpeed;
    // Lane Change Speed
    public float laneChangeSpeed = 20.0f;
    // Jump Variables
    public float jumpForce = 7.0f;
    public float inputThreshold = 0.5f; // This is the threshold for the leftRightInput
    public float groundCheckDistance = 1.5f; // This is the distance to check for the ground
    public float groundCheckStartHeight = .5f; // This is the height to start checking for the ground
    public LayerMask groundLayers; // This is the layer mask for the ground

    public Rigidbody playerRigidbody; // This is the rigidbody component of the player

    private Vector2 leftRightInput; // This is a vector2 that is used to store the value of the leftRightInput
    private bool jumpPressed; // This is a boolean that is used to check if the jump button is pressed

    public int currentLane = 1; // This is the current lane of the player
    private bool canReadLaneInput = true;
// Collision Variables | This is the amount of times the player has collided with an obstacle
    private int collidedAmout = 0;
    public int maxCollisions = 4;
    public float regenTime = 2.0f;

    void Awake(){
        maxSpeed = forwardSpeed * 4;
        minSpeed = forwardSpeed;
        groundLayers = LayerMask.GetMask("Ground"); // Get the layer mask for the ground
        if(playerRigidbody == null){
            playerRigidbody = GetComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnLeftRight(InputValue value){ // This is a function that is called when the leftRightInput is pressed
        leftRightInput = value.Get<Vector2>(); // Get the value of the leftRightInput
        float xInput = leftRightInput.x; // Get the x input
        if(Mathf.Abs(xInput) < inputThreshold){ // If the x input is less than the input threshold, then the player can read the lane input
            canReadLaneInput = true;
            return;
        }
        if(!canReadLaneInput){ // If the player cannot read the lane input, then return
            return;
        }
        if(xInput > 0f){ // If the x input is greater than 0, then the player can move to the right lane
            currentLane++;
        }
        else if(xInput < 0f){ // If the x input is less than 0, then the player can move to the left lane
            currentLane--;
        }
        currentLane = Mathf.Clamp(currentLane, 0, 2); // Clamp the current lane between 0 and 2
        canReadLaneInput = false; // Set the canReadLaneInput to false
    }
    public void OnJump(InputValue value){ // This is a function that is called when the jump button is pressed argument is the value of the input
        if(value.isPressed){ // If the jump button is pressed, then set the jumpPressed to true
            jumpPressed = true;// Set the jumpPressed to true
        }
    }
    void FixedUpdate(){
        float targetX = GetTargetLaneX(); // Get the target x position

        float newX = Mathf.MoveTowards(playerRigidbody.position.x, targetX, laneChangeSpeed * Time.fixedDeltaTime); // Move the player to the target x position by using the MoveTowards function which uses the arguments current position, target position, and the speed
        float xVelocity = (newX - playerRigidbody.position.x) / Time.fixedDeltaTime; // Calculate the x velocity by the difference between the new x position and the current x position divided by the fixed delta time which is the time it takes to complete one frame

        Vector3 velocity = playerRigidbody.linearVelocity; // Get the velocity of the player by getting the velocity of the player's rigidbody
        velocity.x = xVelocity; // Set the x velocity of the player to the x velocity
        velocity.z = forwardSpeed; // Set the z velocity of the player to the forward speed
        playerRigidbody.linearVelocity = velocity; // Set the velocity of the player to the velocity

        // Jump
        if(jumpPressed && IsGrounded()){
            Vector3 jumpVelocity = playerRigidbody.linearVelocity;// Get the velocity of the player by getting the velocity of the player's rigidbody
            jumpVelocity.y = 0f; // Set the y velocity of the player to 0
            playerRigidbody.linearVelocity = jumpVelocity; // Set the velocity of the player to the jump velocity
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Add a force to the player to make them jump by using the AddForce function which uses the arguments force, force mode, force being the force to apply to the player, and the force mode being the mode to apply the force in impulse mode which is a force that is applied instantly
        }
        jumpPressed = false; // Set the jumpPressed to false
    }
    
    float GetTargetLaneX(){ // This is a function that is called to get the target x position of the player
       if(currentLane == 0){ // If the current lane is 0, then return the left lane x position
        return lefLaneX;
       }
       if (currentLane == 1){ // If the current lane is 1, then return the center lane x position
        return centerLaneX;
       }
       return rightLaneX; // If the current lane is 2, then return the right lane x position
    }

    bool IsGrounded(){ // This is a function that is called to check if the player is grounded
        Vector3 rayStart = transform.position + Vector3.up * groundCheckStartHeight; // Get the start of the ray by adding the up vector to the position of the player and the ground check start height
        return Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayers); // Cast a ray down from the start of the ray to the ground check distance and check if the ray hits the ground layers
    }

    #region Lose Life | This is a function that is called to lose a life
    // lose a life function will only be called after player collides with an obstacle x amount of times each collison will cause the camera to shake
    public void LoseLife(){
        if (Time.timeSinceLevelLoad >= 4)
        {
            if (collidedAmout == 0)
            {
                StartCoroutine(RegainLives());
            }
            collidedAmout++; // Increment the collided amount
                             // Debug.Log("Collided Amount: " + collidedAmout); // Log the collided amount
            if (collidedAmout >= maxCollisions)
            { // If the collided amount is greater than or equal to the max collisions
              // Debug.Log("Lives: " + lives);
                collidedAmout = 0; // Reset the collided amount
                                   // call rewind time function
                if (rewind != null)
                {
                    // disable player collider
                    rewind.StartRewind();
                }

            }
            if (gameManager.lives <= 0)
            {
                GameOver();
            }
        }
    }

    IEnumerator RegainLives()
    {
        while (collidedAmout > 0)
        { 
            yield return new WaitForSeconds(regenTime);
            collidedAmout--;
        }
    }

    public void ShakeCamera(int shakeIntensity){
        // shake the camera by using the CinemachineShake script
        CineMachineShake.Instance.ShakeCamera(shakeIntensity);
    } // end of ShakeCamera function
    #endregion
    public void GameOver(){
        SceneManager.LoadScene("LoseScreen");
    }
}
