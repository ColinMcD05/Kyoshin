using System.Collections;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    // Variables
    #region
    // References
    GameObject sectionManager;

    // Array holding to position of the three lanes
    public GameObject[] spawns;
    // static object holding the last obstacle spawned in
    static GameObject lastObject;
    // Vectors holding last and next position
    Vector3 lastPosition;
    Vector3 nextPosition;
    // Holds distance between each obstacle
    float distanceBetween;
    // Check if song is new
    bool isNewSong;
    #endregion

    void Awake()
    {
        sectionManager = GameObject.Find("SectionManager");
    }

    // SpawnObject
    #region
    // Function that spawns in the obstacles
    public void SpawnObject(Transform newParent)
    {
        // Set the range objects can spawn in
        Vector3 farRange = newParent.position;
        Vector3 closeRange = newParent.position;
        
        farRange.z += 16;
        closeRange.z -= 16;

        // Spawns a max of four obstacles
        for (int i = 0; i < 4; i++) 
        { 
            // Set last positiion if last position is not null
            if (lastObject != null)
            {
                lastPosition = lastObject.transform.position;
            }
            if (isNewSong)
            {
                // If song is new, increase next position until it is between far and close range
                nextPosition.z = lastPosition.z + distanceBetween;
                while (nextPosition.z < closeRange.z)
                {
                    nextPosition.z += distanceBetween;
                }
                isNewSong = false;
            }
            else
            {
                // Set next position based on last object position
                nextPosition.z = lastPosition.z + distanceBetween;
            }
            // if next position is between close and far range, spawn in new obstacle
            if (nextPosition.z > closeRange.z && nextPosition.z < farRange.z)
            {
                int randomLane = Random.Range(0, spawns.Length);
                GameObject obstacle = ObjectPool.sharedInstance.GetPooledObstacles();
                if (obstacle != null)
                {
                    obstacle.transform.parent = newParent;
                    //float obstacleWidth = obstacle.GetComponent<MeshRenderer>().bounds.extents.y;
                    // nextPosition.z += obstacleWidth;
                    nextPosition.y = 0.5f;
                    nextPosition.x = spawns[randomLane].transform.position.x;
                    obstacle.transform.position = nextPosition;
                    obstacle.SetActive(true);

                    lastObject = obstacle;
                }
            }
        }
    }
    #endregion

    // ChangeVariables
    #region
    // Function that changes variables
    public void ChangeVariables(Songs.SongData newSong, Vector3 newPosition)
    {
        // Sets the distance between each obstacle
        distanceBetween = 32 * newSong.bps;
        // last position is equal to player posittion at the start of song
        lastPosition = newPosition;
        // Set variables for spawning logic to start correctly
        isNewSong = true;
        lastObject = null;
    }
    #endregion
}
