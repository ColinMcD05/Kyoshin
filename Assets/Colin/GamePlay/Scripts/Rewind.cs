using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rewind : MonoBehaviour
{
    // Variables
    #region
    // References
    [SerializeField] PlayerControllerLevel playerController;
    [SerializeField] Rigidbody playerRigidbody;

    // Mutable Variables in Inspector
    public float rewindTime = 3f; // How far back does the player rewind

    // Mutable Variables in script
    public List<Vector3> positions; // List holding players last known position between 0 and rewindTime seconds

    // Mutable Variables in other scripts
    public bool rewinding = false;
    #endregion

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
        int maxHeld = Mathf.RoundToInt(rewindTime / Time.fixedDeltaTime);

        if (positions.Count > maxHeld)
        {
            positions.RemoveAt(0);
        }
        positions.Add(playerRigidbody.position);
    }
    #endregion

    // RewindTime
    #region
    void RewindTime()
    {
        if (positions.Count > 0)
        {
            int nextPosition = positions.Count - 1;
            playerRigidbody.MovePosition(positions[nextPosition]);
            positions.Remove(positions[nextPosition]);
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
        if (input.isPressed)
        {
            rewinding = !rewinding;
            playerController.enabled = !playerController.enabled;
        }
    }
    #endregion



    // Stop and start Rewind
    #region
    // Lets other scripts more easily start rewind mechanic
    public void StartRewind()
    {
        rewinding = true;
        playerController.enabled = false;
    }

    // Lets other scripts more easily stop rewind mechanic
    public void StopRewind()
    {
        rewinding = false;
        playerController.enabled = true;
    }
    #endregion
}
