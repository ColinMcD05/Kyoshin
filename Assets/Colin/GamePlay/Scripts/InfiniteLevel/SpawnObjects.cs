using System.Collections;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    // Variables
    #region
    // References
    SectionManager sectionManager;

    // Array holding to position of the three lanes
    public GameObject[] spawns;
    // static object holding the last obstacle spawned in
    static GameObject lastObject;
    static float lastObstacleWidth;
    // Vectors holding last and next position
    Vector3 lastPosition;
    Vector3 nextPosition;

    // Wall Positions
    public Vector3 closeLeftWall;
    public Vector3 closeRightWall;
    public Vector3 leftWall;
    public Vector3 rightWall;

    // Holds distance between each obstacle
    float distanceBetween;
    // Check if song is new
    bool isNewSong;

 

    // Enumerator used to determine which obstacle types can be spawned
    public enum ObstacleLaneType
    {
        SingleObject,
        LeftRight,
        LeftMiddle,
        RightMiddle,
        ThreeLanes,
        LeftWall,
        RightWall
    }

    // Which objects to spawn based on level
    public enum Level
    {
        All,
        Hakone,
        Kyoto,
        Tokyo
    }

    #endregion

    void Awake()
    {
        sectionManager = GameObject.Find("SectionManager").GetComponent<SectionManager>();
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

        float obstacleWidth = 0;

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
                GameObject obstacle = ObjectPool.sharedInstance.GetPooledObstacles(lastObject);
                if (obstacle != null)
                {
                    // Get the obstacle script to get the type of area it is in
                    Obstacle obstacleScript = obstacle.GetComponent<Obstacle>();
                    obstacle.SetActive(true);
                    obstacle.transform.parent = newParent;

                    int firstObject = 0;
                    int secondObject = 0;
                    int thirdObject = 0;

                    switch (obstacleScript.obstacleLaneType)
                    {
                        default:
                        case ObstacleLaneType.SingleObject:
                            switch (sectionManager.currentArea)
                            {
                                case SectionManager.AreaType.AllOpen:
                                case SectionManager.AreaType.WallRun:
                                case SectionManager.AreaType.CloseWallRun:
                                    break;
                                case SectionManager.AreaType.LeftClosed:
                                    if (randomLane == 0)
                                    {
                                        randomLane = Random.Range(1, spawns.Length);
                                    }
                                    break;
                                case SectionManager.AreaType.MiddleClosed:
                                    while (randomLane == 1)
                                    {
                                        randomLane = Random.Range(1, spawns.Length);
                                    }
                                    break;
                                case SectionManager.AreaType.RightClosed:
                                    if (randomLane == 2)
                                    {
                                        randomLane = Random.Range(0, 2);
                                    }
                                    break;
                            }
                            obstacleWidth = obstacle.GetComponent<Collider>().bounds.extents.z;
                            nextPosition.x = spawns[randomLane].transform.position.x;
                            break;
                        case ObstacleLaneType.LeftRight:
                            firstObject = Random.Range(0, obstacle.transform.childCount);
                            secondObject = Random.Range(0, obstacle.transform.childCount);
                            while (secondObject == firstObject)
                            {
                                secondObject = Random.Range(0, obstacle.transform.childCount);
                            }
                            obstacle.transform.GetChild(firstObject).gameObject.SetActive(true);
                            obstacle.transform.GetChild(firstObject).transform.position = new Vector3(spawns[0].transform.position.x, 0, 0);
                            obstacle.transform.GetChild(secondObject).gameObject.SetActive(true);
                            obstacle.transform.GetChild(secondObject).transform.position = new Vector3(spawns[2].transform.position.x, 0, 0);
                            obstacleWidth = obstacle.transform.GetChild(firstObject).GetComponent<Collider>().bounds.extents.z;
                            nextPosition.x = 0;
                            break;
                        case ObstacleLaneType.LeftMiddle:
                            firstObject = Random.Range(0, obstacle.transform.childCount);
                            secondObject = Random.Range(0, obstacle.transform.childCount);
                            while (secondObject == firstObject)
                            {
                                secondObject = Random.Range(0, obstacle.transform.childCount);
                            }
                            obstacle.transform.GetChild(firstObject).gameObject.SetActive(true);
                            obstacle.transform.GetChild(firstObject).transform.position = new Vector3(spawns[0].transform.position.x, 0, 0);
                            obstacle.transform.GetChild(secondObject).gameObject.SetActive(true);
                            obstacle.transform.GetChild(secondObject).transform.position = new Vector3(spawns[1].transform.position.x, 0, 0);
                            obstacleWidth = obstacle.transform.GetChild(firstObject).GetComponent<Collider>().bounds.extents.z;
                            nextPosition.x = 0;
                            break;
                        case ObstacleLaneType.RightMiddle:
                            firstObject = Random.Range(0, obstacle.transform.childCount);
                            secondObject = Random.Range(0, obstacle.transform.childCount);
                            while (secondObject == firstObject)
                            {
                                secondObject = Random.Range(0, obstacle.transform.childCount);
                            }
                            obstacle.transform.GetChild(firstObject).gameObject.SetActive(true);
                            obstacle.transform.GetChild(firstObject).transform.position = new Vector3(spawns[1].transform.position.x, 0, 0);
                            obstacle.transform.GetChild(secondObject).gameObject.SetActive(true);
                            obstacle.transform.GetChild(secondObject).transform.position = new Vector3(spawns[2].transform.position.x, 0, 0);
                            obstacleWidth = obstacle.transform.GetChild(firstObject).GetComponent<Collider>().bounds.extents.z;
                            nextPosition.x = 0;
                            break;
                        case ObstacleLaneType.ThreeLanes:
                            if (obstacle.transform.childCount > 1)
                            {
                                firstObject = Random.Range(0, obstacle.transform.childCount);
                                secondObject = Random.Range(0, obstacle.transform.childCount);
                                while (secondObject == firstObject)
                                {
                                    secondObject = Random.Range(0, obstacle.transform.childCount);
                                }
                                thirdObject = Random.Range(0, obstacle.transform.childCount);
                                while (thirdObject == secondObject || thirdObject == firstObject)
                                {
                                    thirdObject = Random.Range(0, obstacle.transform.childCount);
                                }
                                obstacle.transform.GetChild(firstObject).gameObject.SetActive(true);
                                obstacle.transform.GetChild(firstObject).transform.position = new Vector3(spawns[0].transform.position.x, 0, 0);
                                obstacle.transform.GetChild(secondObject).gameObject.SetActive(true);
                                obstacle.transform.GetChild(secondObject).transform.position = new Vector3(spawns[1].transform.position.x, 0, 0);
                                obstacle.transform.GetChild(thirdObject).gameObject.SetActive(true);
                                obstacle.transform.GetChild(thirdObject).transform.position = new Vector3(spawns[2].transform.position.x, 0, 0);
                                obstacleWidth = obstacle.transform.GetChild(firstObject).GetComponent<Collider>().bounds.extents.z;
                            }
                            else
                            {
                                nextPosition.x = 0;
                                obstacleWidth = obstacle.GetComponent<Collider>().bounds.extents.z;
                            }
                            break;
                        case ObstacleLaneType.LeftWall:
                            switch (sectionManager.currentArea)
                            {
                                default:
                                case SectionManager.AreaType.CloseWallRun:
                                    nextPosition.y = closeLeftWall.y;
                                    nextPosition.x = closeLeftWall.x;
                                    break;
                                case SectionManager.AreaType.WallRun:
                                    nextPosition.y = closeLeftWall.y;
                                    nextPosition.x = closeLeftWall.x;
                                    break;
                            }
                            break;
                        case ObstacleLaneType.RightWall:
                            switch (sectionManager.currentArea)
                            {
                                default:
                                case SectionManager.AreaType.CloseWallRun:
                                    nextPosition.y = closeRightWall.y;
                                    nextPosition.x = closeRightWall.x;
                                    break;
                                case SectionManager.AreaType.WallRun:
                                    nextPosition.y = closeRightWall.y;
                                    nextPosition.x = closeRightWall.x;
                                    break;
                            }
                            break;
                    }

                    if (lastObject != null)
                    {

                        float changeBasedOnWidth =  lastObstacleWidth-obstacleWidth;
                        //Debug.Log(changeBasedOnWidth);

                        nextPosition.z += changeBasedOnWidth;
                    }
                    else
                    {
                        nextPosition.z -= obstacleWidth;
                    }
                    if (obstacleScript.obstacleLaneType != ObstacleLaneType.LeftWall && obstacleScript.obstacleLaneType != ObstacleLaneType.RightWall)
                    {
                        nextPosition.y = 0.5f;
                    }
                    obstacle.transform.position = nextPosition;

                    lastObject = obstacle;
                    lastObstacleWidth = obstacleWidth;
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
        switch (newSong.levelName)
        {
            default:
                ObjectPool.sharedInstance.currentLevel = Level.All;
                break;
            case "Kyoto":
                ObjectPool.sharedInstance.currentLevel = Level.Kyoto;
                break;
            case "Tokyo":
                ObjectPool.sharedInstance.currentLevel = Level.Tokyo;
                break;
            case "Hakone":
                ObjectPool.sharedInstance.currentLevel = Level.Hakone;
                break;
        }
    }
    #endregion
}
