using System.Collections;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    GameObject sectionManager;
    public GameObject[] spawns;

    static GameObject lastObject;
    Vector3 lastPosition;
    Vector3 nextPosition;
    float distanceBetween;
    bool isNewSong;
    public float buffer = 2;

    void Awake()
    {
        sectionManager = GameObject.Find("SectionManager");
    }

    public void SpawnObject(Transform newParent)
    {
        Vector3 farRange = newParent.position;
        Vector3 closeRange = newParent.position;

        farRange.z += 16;
        closeRange.z -= 16;

        for (int i = 0; i < 4; i++) 
        { 
            if (lastObject != null)
            {
                lastPosition = lastObject.transform.position;
            }
            if (isNewSong)
            {
                nextPosition.z = lastPosition.z + distanceBetween;
                while (nextPosition.z < closeRange.z)
                {
                    nextPosition.z += distanceBetween;
                }
                isNewSong = false;
            }
            else
            {
                nextPosition.z = lastPosition.z + distanceBetween;
            }
            if (nextPosition.z > closeRange.z && nextPosition.z < farRange.z)
            {
                int randomLane = Random.Range(0, spawns.Length);
                GameObject obstacle = ObjectPool.sharedInstance.GetPooledObstacles();
                if (obstacle != null)
                {
                    obstacle.transform.parent = newParent;
                    float obstacleWidth = obstacle.GetComponent<MeshRenderer>().bounds.extents.y;
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

    public void ChangeVariables(Songs.SongData newSong, Vector3 newPosition)
    {
        distanceBetween = 32 * newSong.bps;
        lastPosition = newPosition;
        isNewSong = true;
        lastObject = null;
    }
}
