using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static SpawnObjects;

public class ObjectPool : MonoBehaviour
{
    // Variables
    #region
    public static ObjectPool sharedInstance;
    [Header("Pooled Objects")]
    public List<GameObject> pooledSections, pooledKyotoObstacles, pooledTokyoObstacles, pooledHakoneObstacles;

    [Header("Section Info")]
    public GameObject sectionToPool;
    public int amountSectionsToPool;

    [Header("Obstacles Info")]
    [Header("Kyoto Objects")]
    public GameObject[] KyotoToPool;
    public int[] amountKyotoToPool;
    Dictionary<SpawnObjects.ObstacleLaneType, int> kyotoTypeRanges;
    [Header("Tokyo Objects")]
    public GameObject[] TokyoToPool;
    public int[] amountTokyoToPool;
    Dictionary<SpawnObjects.ObstacleLaneType, int> tokyoTypeRanges;
    [Header("Hakone Objects")]
    public GameObject[] HakoneToPool;
    public int[] amountHakoneToPool;
    Dictionary<SpawnObjects.ObstacleLaneType, int> hakoneTypeRanges;
    #endregion

    public SpawnObjects.Level currentLevel;

    // Awake
    #region
    void Awake()
    {
        // Sets shared instance to this instance of game object
        sharedInstance = this;

        // Creating an object pool for all the sections
        pooledSections = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountSectionsToPool; i++)
        {
            tmp = Instantiate(sectionToPool);
            tmp.SetActive(false);
            pooledSections.Add(tmp);
        }

        int startPosition = 0;
        // Creating an object pool for kyoto obstacles
        pooledKyotoObstacles = new List<GameObject>();
        kyotoTypeRanges = new Dictionary<ObstacleLaneType, int>();
        for (int i = 0; i < amountKyotoToPool.Length; i++)
        {
            for (int j = 0; j < amountKyotoToPool[i]; j++)
            {
                tmp = Instantiate(KyotoToPool[i]);
                ObstacleLaneType laneType = tmp.GetComponent<Obstacle>().obstacleLaneType;
                try 
                { 
                    kyotoTypeRanges.Add(laneType, startPosition); 
                }
                catch (ArgumentException)
                {
                }
                startPosition++;
                tmp.SetActive(false);
                pooledKyotoObstacles.Add(tmp);
            }
        }

        startPosition = 0;
        // Creating an object pool for Hakone obstacles
        pooledHakoneObstacles = new List<GameObject>();
        hakoneTypeRanges = new Dictionary<ObstacleLaneType, int>();
        for (int i = 0; i < amountHakoneToPool.Length; i++)
        {
            for (int j = 0; j < amountHakoneToPool[i]; j++)
            {
                tmp = Instantiate(HakoneToPool[i]);
                ObstacleLaneType laneType = tmp.GetComponent<Obstacle>().obstacleLaneType;
                try
                {
                    hakoneTypeRanges.Add(laneType, startPosition);
                }
                catch (ArgumentException)
                {              
                }
                startPosition++;
                tmp.SetActive(false);
                pooledHakoneObstacles.Add(tmp);
            }
        }

        startPosition = 0;
        // Creating an object pool for Tokyo obstacles
        pooledTokyoObstacles = new List<GameObject>();
        tokyoTypeRanges = new Dictionary<ObstacleLaneType, int>();
        for (int i = 0; i < amountTokyoToPool.Length; i++)
        {
            for (int j = 0; j < amountTokyoToPool[i]; j++)
            {
                tmp = Instantiate(TokyoToPool[i]);
                ObstacleLaneType laneType = tmp.GetComponent<Obstacle>().obstacleLaneType;
                try
                {
                    tokyoTypeRanges.Add(laneType, startPosition);
                }
                catch
                {
                }
                startPosition++;
                tmp.SetActive(false);
                pooledTokyoObstacles.Add(tmp);
            }
        }
    }
    #endregion

    // Get Pooled Objects
    #region
    // Get a section from the object pool
    public GameObject GetPooledSections()
    {
        // Check for the first section that is not active in the hierarchy and return it
        for (int i = 0; i < amountSectionsToPool; i++)
        {
            if (!pooledSections[i].activeInHierarchy)
            {
                return pooledSections[i];
            }
        }
        return null;
    }

    // Need to limit range by finding area type and last pooled objects lane type
    // Get Obstacle from the object pool
    public GameObject GetPooledObstacles(GameObject lastObstacles)
    {
        List<ObstacleLaneType> unavailableLanes = new List<ObstacleLaneType>();

        if (lastObstacles != null)
        {
            ObstacleLaneType lastLane = lastObstacles.GetComponent<Obstacle>().obstacleLaneType;
            switch (lastLane)
            {
                default:
                case ObstacleLaneType.SingleObject:
                    break;
                case ObstacleLaneType.LeftMiddle:
                    unavailableLanes.Add(ObstacleLaneType.RightMiddle);
                    unavailableLanes.Add(ObstacleLaneType.ThreeLanes); 
                    break;
                case ObstacleLaneType.LeftRight:
                    unavailableLanes.Add(ObstacleLaneType.ThreeLanes);
                    break;
                case ObstacleLaneType.RightMiddle:
                    unavailableLanes.Add(ObstacleLaneType.LeftMiddle);
                    unavailableLanes.Add(ObstacleLaneType.ThreeLanes);
                    break;
                case ObstacleLaneType.ThreeLanes:
                    unavailableLanes.Add(ObstacleLaneType.LeftMiddle);
                    unavailableLanes.Add(ObstacleLaneType.RightMiddle);
                    break;
                case ObstacleLaneType.LeftWall:
                    break;
                case ObstacleLaneType.RightWall:
                    break;
            }
        }
        return null;
    }
    #endregion
}
