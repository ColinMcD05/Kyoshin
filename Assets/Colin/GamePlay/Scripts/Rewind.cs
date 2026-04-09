using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rewind : MonoBehaviour
{
    // Variables
    #region
    // References
    GameManager gameManager;
    [SerializeField] PlayerControllerLevel playerController;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] Timing timing;
    [SerializeField] Collider playerCollider;

    // Mutable Variables in Inspector
    public float rewindTime = 3f; // How far back does the player rewind

    // Mutable Variables in script
    public List<Vector3> positions; // List holding players last known position between 0 and rewindTime seconds

    // Mutable Variables in other scripts
    public bool rewinding = false;
    #endregion

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        if (positions.Count > maxHeld)
        {
            positions.RemoveAt(0); // Remove the first position if list is greater than max held
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
            StartRewind();
        }
    }
    #endregion



    // Stop and start Rewind
    #region
    // Lets other scripts more easily start rewind mechanic
    public void StartRewind()
    {
        rewinding = true;

        musicPlayer.pitch = -1; // Reverses music

        // Disables parts of player
        playerController.enabled = false;
        playerCollider.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        // Lose a life when rewinding
        gameManager.lives--;
    }

    // Lets other scripts more easily stop rewind mechanic
    public void StopRewind()
    {
        rewinding = false;

        musicPlayer.pitch = 1; // Music plays normally
        timing.rewindTimeUsed += rewindTime; // Adds time that was rewound to get accurate position of song
        
        // Enables parts of player
        GetComponent<Rigidbody>().isKinematic = false;
        playerCollider.enabled = true;
        playerController.enabled = true;
    }
    #endregion
}
