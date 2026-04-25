using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    // Variables
    #region
    public static ObjectPool sharedInstance;
    [Header("Pooled Objects")]
    public List<GameObject> pooledSections, pooledObstacles;

    [Header("Section Info")]
    public GameObject sectionToPool;
    public int amountSectionsToPool;

    [Header("Obstacles Info")]
    public GameObject[] obstaclesToPool;
    public int[] amountObstaclesToPool;
    #endregion

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

        // Creating an object pool for all obstacles
        pooledObstacles = new List<GameObject>();
        for (int i = 0; i < amountObstaclesToPool.Length; i++)
        {
            for (int j = 0; j < amountObstaclesToPool[i]; j++)
            {
                tmp = Instantiate(obstaclesToPool[i]);
                tmp.SetActive(false);
                pooledObstacles.Add(tmp);
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

    // Get Obstacle from the object pool
    public GameObject GetPooledObstacles()
    {
        // Check for the first obstacle that is not active in the hierarchy and return it
        for (int i = 0; i < amountObstaclesToPool[0]; i++)
        {
            if (!pooledObstacles[i].activeInHierarchy)
            {
                return pooledObstacles[i];
            }
        }
        return null;
    }
    #endregion
}
