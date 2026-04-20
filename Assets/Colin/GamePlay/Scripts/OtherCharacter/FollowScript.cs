using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    GameObject player;
    Rewind playerRewind;
    Rigidbody charRigidbody;
    Vector3 charPosition;

    public List<Vector3> positionsLog;
    float rewindTime;

    public int spacing = 5;

    void Start()
    {
        player = GameObject.Find("Player");
        playerRewind = player.GetComponent<Rewind>();
        charRigidbody = GetComponent<Rigidbody>();
        charPosition.z = transform.position.z;
        rewindTime = playerRewind.rewindTime;
    }

    private void FixedUpdate()
    {
        if (!playerRewind.rewinding)
        {
            Follow();
            RecordPos();
        }
        else
        {
            RewindTime();
        }
    }

    private void Follow()
    {
        if (playerRewind.positions.Count < spacing)
        {
            return;
        }
        else
        {
            int nextPosition = playerRewind.positions.Count - spacing;
            charPosition.x = playerRewind.positions[nextPosition].x;
            charPosition.y = playerRewind.positions[nextPosition].y;
            charRigidbody.MovePosition(charPosition);
        }
    }

    private void RecordPos()
    {
        int maxHeld = Mathf.RoundToInt(rewindTime / Time.fixedDeltaTime); // Variable used to incicate how many positions can be held

        positionsLog.Add(charRigidbody.position); // Adds current player position to the list
        if (positionsLog.Count > maxHeld)
        {
            positionsLog.RemoveAt(0); // Remove the first position if list is greater than max held
        }
    }

    void RewindTime()
    {
        if (positionsLog.Count > 0) // Checks if there are still places to go
        {
            int nextPosition = positionsLog.Count - 1; // Gets last position in list index
            charRigidbody.MovePosition(positionsLog[nextPosition]); // Moves player to last position in list index
            positionsLog.Remove(positionsLog[nextPosition]); // Removes last position from list index
        }
    }
}
