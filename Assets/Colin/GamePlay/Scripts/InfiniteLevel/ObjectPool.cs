using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static SpawnObjects;

public class ObjectPool : MonoBehaviour
{
    // Variables
    #region
    [SerializeField] SectionManager sectionManager;

    public static ObjectPool sharedInstance;
    [Header("Pooled Objects")]
    public List<GameObject> pooledKyotoSections, pooledHakoneSections, pooledTokyoSections;
    Dictionary<ObstacleLaneType, List<GameObject>> pooledKyotoObstacles, pooledTokyoObstacles, pooledHakoneObstacles;

    [Header("Section Info")]
    public GameObject[] sectionKyotoPool;
    public int[] amountSectionKyotoPool;
    public GameObject[] sectionHakonePool;
    public int[] amountSectionHakonePool;
    public GameObject[] sectionTokyoPool;
    public int[] amountSectionTokyoPool;

    [Header("Obstacles Info")]
    [Header("Kyoto Objects")]
    public GameObject[] KyotoToPool;
    public int[] amountKyotoToPool;
    [Header("Tokyo Objects")]
    public GameObject[] TokyoToPool;
    public int[] amountTokyoToPool;
    [Header("Hakone Objects")]
    public GameObject[] HakoneToPool;
    public int[] amountHakoneToPool;

    public SpawnObjects.Level currentLevel;
    public SectionManager.AreaType aeraType;

    /// <summary>
    /// 0 Kyoto, 1 Hakone, 2 Tokyo
    /// </summary>
    [Header("Skyboxes")]
    public Material[] skyBoxes;
    #endregion

    // Awake
    #region
    void Awake()
    {
        // Sets shared instance to this instance of game object
        sharedInstance = this;

        // Creating an object pool for the Kyoto sections
        pooledKyotoSections = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountSectionKyotoPool.Length; i++)
        {
            for (int j = 0; j < amountSectionKyotoPool[i]; j++)
            {
                tmp = Instantiate(sectionKyotoPool[i]);
                tmp.SetActive(false);
                pooledKyotoSections.Add(tmp);
            }
        }

        // Creating an object pool for all the Hakone sections
        pooledHakoneSections = new List<GameObject>();
        for (int i = 0; i < amountSectionHakonePool.Length; i++)
        {
            for (int j = 0; j < amountSectionHakonePool[i]; j++)
            {
                tmp = Instantiate(sectionHakonePool[i]);
                tmp.SetActive(false);
                pooledHakoneSections.Add(tmp);
            }
        }

        // Creating an object pool for all the Tokyo sections
        pooledTokyoSections = new List<GameObject>();
        for (int i = 0; i < amountSectionTokyoPool.Length; i++)
        {
            for (int j = 0; j < amountSectionTokyoPool[i]; j++)
            {
                tmp = Instantiate(sectionTokyoPool[i]);
                tmp.SetActive(false);
                pooledTokyoSections.Add(tmp);
            }
        }

        // Creating an object pool for kyoto obstacles
        pooledKyotoObstacles = new Dictionary<ObstacleLaneType, List<GameObject>>();
        for (int i = 0; i < amountKyotoToPool.Length; i++)
        {
            List<GameObject> pooledObstacles = new List<GameObject>();
            ObstacleLaneType laneType = KyotoToPool[i].GetComponent<Obstacle>().obstacleLaneType;
            for (int j = 0; j < amountKyotoToPool[i]; j++)
            {
                tmp = Instantiate(KyotoToPool[i]);
                tmp.SetActive(false);
                pooledObstacles.Add(tmp);
            }
            try
            {
                pooledKyotoObstacles.Add(laneType, pooledObstacles);
            }
            catch (ArgumentException)
            {
                pooledKyotoObstacles[laneType].AddRange(pooledObstacles);
            }
        }

        // Creating an object pool for Hakone obstacles
        pooledHakoneObstacles = new Dictionary<ObstacleLaneType, List<GameObject>>();
        for (int i = 0; i < amountHakoneToPool.Length; i++)
        {
            List<GameObject> pooledObstacles = new List<GameObject>();
            ObstacleLaneType laneType = HakoneToPool[i].GetComponent<Obstacle>().obstacleLaneType;
            for (int j = 0; j < amountHakoneToPool[i]; j++)
            {
                tmp = Instantiate(HakoneToPool[i]);
                tmp.SetActive(false);
                pooledObstacles.Add(tmp);
            }
            try
            {
                pooledHakoneObstacles.Add(laneType, pooledObstacles);
            }
            catch (ArgumentException)
            {
                pooledHakoneObstacles[laneType].AddRange(pooledObstacles);
            }
        }

        // Creating an object pool for Tokyo obstacles
        pooledTokyoObstacles = new Dictionary<ObstacleLaneType, List<GameObject>>();
        for (int i = 0; i < amountTokyoToPool.Length; i++)
        {
            List<GameObject> pooledObstacles = new List<GameObject>();
            ObstacleLaneType laneType = TokyoToPool[i].GetComponent<Obstacle>().obstacleLaneType;
            for (int j = 0; j < amountTokyoToPool[i]; j++)
            {
                tmp = Instantiate(TokyoToPool[i]);
                tmp.SetActive(false);
                pooledObstacles.Add(tmp);
            }
            try
            {
                pooledTokyoObstacles.Add(laneType, pooledObstacles);
            }
            catch (ArgumentException)
            {
                pooledTokyoObstacles[laneType].AddRange(pooledObstacles);
            }
        }
    }
    #endregion

    // Get Pooled Objects
    #region
    // Get a section from the object pool
    public GameObject GetPooledSections()
    {
        int randomSection = 0;
        switch (currentLevel)
        {
            default:
            case Level.All:
                break;
            case Level.Kyoto:
                randomSection = UnityEngine.Random.Range(0, pooledKyotoSections.Count);
                while (pooledKyotoSections[randomSection].activeInHierarchy)
                {
                    randomSection = randomSection = UnityEngine.Random.Range(0, pooledKyotoSections.Count);
                }
                return pooledHakoneSections[randomSection];
            case Level.Hakone:
                randomSection = UnityEngine.Random.Range(0, pooledHakoneSections.Count);
                while (pooledHakoneSections[randomSection].activeInHierarchy)
                {
                    randomSection = randomSection = UnityEngine.Random.Range(0, pooledHakoneSections.Count);
                }
                return pooledHakoneSections[randomSection];
            case Level.Tokyo:
                randomSection = UnityEngine.Random.Range(0, pooledTokyoSections.Count);
                while (pooledTokyoSections[randomSection].activeInHierarchy)
                {
                    randomSection = randomSection = UnityEngine.Random.Range(0, pooledTokyoSections.Count);
                }
                return pooledTokyoSections[randomSection];
        }
        return null;
    }

    // Need to limit range by finding area type and last pooled objects lane type
    // Get Obstacle from the object pool
    public GameObject GetPooledObstacles(GameObject lastObstacles)
    {
        HashSet<ObstacleLaneType> unavailableLanes = new HashSet<ObstacleLaneType>();
        List<GameObject> availableObstacles = new List<GameObject>();
        int numbersToRandomize = 0;

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
                    unavailableLanes.Add(ObstacleLaneType.ThreeLanes);
                    break;
                case ObstacleLaneType.LeftWall:
                    break;
                case ObstacleLaneType.RightWall:
                    break;
            }
        }
        switch (sectionManager.currentArea)
        {
            default:
            case SectionManager.AreaType.AllOpen:
                unavailableLanes.Add(ObstacleLaneType.LeftWall);
                unavailableLanes.Add(ObstacleLaneType.RightWall);
                break;
            case SectionManager.AreaType.WallRun:
                unavailableLanes.Add(ObstacleLaneType.LeftWall);
                unavailableLanes.Add(ObstacleLaneType.RightWall);
                break;
            case SectionManager.AreaType.CloseWallRun:
                unavailableLanes.Add(ObstacleLaneType.LeftMiddle);
                unavailableLanes.Add(ObstacleLaneType.RightMiddle);
                unavailableLanes.Add(ObstacleLaneType.LeftRight);
                unavailableLanes.Add(ObstacleLaneType.ThreeLanes);
                break;
            case SectionManager.AreaType.RightClosed:
                unavailableLanes.Add(ObstacleLaneType.LeftRight);
                unavailableLanes.Add(ObstacleLaneType.LeftMiddle);
                unavailableLanes.Add(ObstacleLaneType.RightMiddle);
                unavailableLanes.Add(ObstacleLaneType.ThreeLanes);
                unavailableLanes.Add(ObstacleLaneType.LeftWall);
                unavailableLanes.Add(ObstacleLaneType.RightWall);
                break;
            case SectionManager.AreaType.LeftClosed:
                unavailableLanes.Add(ObstacleLaneType.LeftRight);
                unavailableLanes.Add(ObstacleLaneType.LeftMiddle);
                unavailableLanes.Add(ObstacleLaneType.RightMiddle);
                unavailableLanes.Add(ObstacleLaneType.ThreeLanes);
                unavailableLanes.Add(ObstacleLaneType.LeftWall);
                unavailableLanes.Add(ObstacleLaneType.RightWall);
                break;
        }
        // Looks at level
        switch (currentLevel)
        {
            default:
            case Level.All:
                foreach(ObstacleLaneType key in pooledKyotoObstacles.Keys)
                {
                    if (!unavailableLanes.Contains(key))
                    {
                        availableObstacles.AddRange(pooledKyotoObstacles[key]);
                    }
                }
                foreach (ObstacleLaneType key in pooledHakoneObstacles.Keys)
                {
                    if (!unavailableLanes.Contains(key))
                    {
                        availableObstacles.AddRange(pooledHakoneObstacles[key]);
                    }
                }
                foreach (ObstacleLaneType key in pooledTokyoObstacles.Keys)
                {
                    if (!unavailableLanes.Contains(key))
                    {
                        availableObstacles.AddRange(pooledTokyoObstacles[key]);
                    }
                }
                break;
            case Level.Hakone:
                foreach (ObstacleLaneType key in pooledHakoneObstacles.Keys)
                {
                    if (!unavailableLanes.Contains(key))
                    {
                        availableObstacles.AddRange(pooledHakoneObstacles[key]);
                    }
                }
                break;
            case Level.Kyoto:
                foreach (ObstacleLaneType key in pooledKyotoObstacles.Keys)
                {
                    if (!unavailableLanes.Contains(key))
                    {
                        availableObstacles.AddRange(pooledKyotoObstacles[key]);
                    }
                }
                break;
            case Level.Tokyo:
                foreach (ObstacleLaneType key in pooledTokyoObstacles.Keys)
                {
                    if (!unavailableLanes.Contains(key))
                    {
                        availableObstacles.AddRange(pooledTokyoObstacles[key]);
                    }
                }
                break;
        }
        numbersToRandomize = availableObstacles.Count;

        int randomObject = UnityEngine.Random.Range(0, numbersToRandomize);
        while (availableObstacles[randomObject].activeInHierarchy)
        {
            randomObject = UnityEngine.Random.Range(0, numbersToRandomize);
        }

        if (availableObstacles[randomObject].GetComponent<Obstacle>().areaType != SectionManager.AreaType.AllOpen)
        {
            aeraType = availableObstacles[randomObject].GetComponent<Obstacle>().areaType;
        }

        return availableObstacles[randomObject];
    }
    #endregion
}
