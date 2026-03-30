using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewind : MonoBehaviour
{
    // Variables
    #region
    // Mutable Variables in Inspector
    public float rewindTime = 3f; // How far back does the player rewind

    // Mutable Variables in script
    public List<Vector3> positions; // List holding players last known position between 0 and rewindTime seconds

    // Mutable Variables in other scripts
    public bool rewinding = false;
    #endregion

    // Awake Instances
    #region
    void Awake()
    {
        StartCoroutine(RecordPos());
    }
    #endregion

    // Update
    #region
    void Update()
    {
        if (rewinding)
        {
            RewindTime();
        }
    }
    #endregion

    // RecordPos
    #region
    // Records the position of the player
    private IEnumerator RecordPos()
    {
        float time = 0;
        while (1 == 1)
        {
            if (!rewinding)
            {
                if (time < rewindTime) // starts filling up the position list once game starts
                {
                    time += Time.deltaTime;
                    positions.Add(transform.position);
                }
                else // after rewindTime amount of seconds removes the first index in the list and keeps adding in new ones.
                {
                    positions.Remove(positions[0]);
                    positions.Add(transform.position);
                }
            }
            if (rewinding && time != 0)
            {
                time = 0;
            }
            yield return null;
        }
    }
    #endregion

    // RewindTime
    #region
    void RewindTime()
    {
        if (positions.Count > 0)
        {
            int nextPosition = positions.Count - 1;
            transform.position = positions[nextPosition];
            positions.Remove(positions[nextPosition]);
        }
        else
        {
            rewinding = false;
        }
    }
    #endregion
}
