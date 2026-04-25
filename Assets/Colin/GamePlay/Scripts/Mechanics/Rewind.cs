using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Rewind : MonoBehaviour
{
    // Variables
    #region
    // References
    GameManager gameManager;
    [SerializeField] PlayerControllerLevel playerController;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] PlayerLevelMovement playerMovement;
    [SerializeField] MoveBackwards moveBackwards;
    AudioSource musicPlayer;
    [SerializeField] Timing timing;
    [SerializeField] Collider playerCollider;

    // Mutable Variables in Inspector
    public float rewindTime = 3f; // How far back does the player rewind
    public float invincibility = 2f;

    // Mutable Variables in script
    [HideInInspector] public List<Vector2> positions; // List holding players last known position between 0 and rewindTime seconds
    [HideInInspector] public List<int> lane;
    [HideInInspector] public List<int> laneSpeed;
    float startRewindTime;
    float totalRewindTime;

    // Mutable Variables in other scripts
    public bool rewinding = false;
    #endregion

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        musicPlayer = GameObject.Find("Audio").transform.Find("Music").GetComponent<AudioSource>();
    }

    // FixedUpdate
    #region
    void FixedUpdate()
    {
        if (rewinding)
        {
            RewindTime();
        }
        else
        {
            RecordPos();
        }
    }
    #endregion

    // RecordPos
    #region
    // Records the position of the player
    private void RecordPos()
    {
        int maxHeld = Mathf.RoundToInt(rewindTime / Time.fixedDeltaTime); // Variable used to incicate how many positions can be held

        positions.Add(playerRigidbody.position); // Adds current player position to the list
        lane.Add(playerMovement.currentLane);
        laneSpeed.Add(moveBackwards.forwardSpeed);
        if (positions.Count > maxHeld)
        {
            positions.RemoveAt(0); // Remove the first position if list is greater than max held
            lane.RemoveAt(0);
            laneSpeed.RemoveAt(0);
        }
    }
    #endregion

    // RewindTime
    #region
    void RewindTime()
    {
        if (positions.Count > 0) // Checks if there are still places to go
        {
            int nextPosition = positions.Count - 1; // Gets last position in list index
            playerRigidbody.MovePosition(positions[nextPosition]); // Moves player to last position in list index
            positions.Remove(positions[nextPosition]); // Removes last position from list index

            nextPosition = laneSpeed.Count - 1;
            moveBackwards.forwardSpeed = laneSpeed[nextPosition] * -1;
            laneSpeed.RemoveAt(nextPosition);
        }
        else
        {
            StopRewind();
        }
    }
    #endregion

    // OnRewind
    #region
    // Function gets called when the rewind button is pressed
    public void OnRewind(InputValue input)
    {
        // Rewind starts if not currently rewinding and enough time has passed
        if (input.isPressed && !rewinding && Time.timeSinceLevelLoad >=4)
        {
            playerController.Death();
        }
    }
    #endregion



    // Stop and start Rewind
    #region
    // Lets other scripts more easily start rewind mechanic
    public void StartRewind()
    {
        startRewindTime = (float)AudioSettings.dspTime; ;
        rewinding = true;

        musicPlayer.pitch = -1; // Reverses music
        moveBackwards.forwardSpeed *= -1;

        // Disables parts of player
        playerController.enabled = false;
        playerMovement.UnSubscribeActions();
        timing.UnSubscribeActions();
        Time.timeScale = 2;
    }

    // Lets other scripts more easily stop rewind mechanic
    public void StopRewind()
    {
        totalRewindTime = (float)AudioSettings.dspTime - startRewindTime;
        rewinding = false;

        musicPlayer.pitch = 1; // Music plays normally
        timing.rewindTimeUsed += totalRewindTime; // Adds time that was rewound to get accurate position of song
        Debug.Log(totalRewindTime);

        // Enables parts of player
        Invoke("BecomeVulnerable", invincibility);
        timing.SubscribeActions();
        playerMovement.currentLane = lane[0];
        moveBackwards.forwardSpeed *= -1;
        moveBackwards.forwardSpeed = moveBackwards.minSpeed;
        lane.Clear();
        Time.timeScale = 1;
    }
    #endregion

    public void BecomeVulnerable()
    {
        playerController.enabled = true;
    }
}
