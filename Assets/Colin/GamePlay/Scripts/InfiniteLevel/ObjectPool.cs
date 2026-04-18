using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool sharedInstance;
    [Header("Pooled Objects")]
    public List<GameObject> pooledSections, pooledObstacles;

    [Header("Section Info")]
    public GameObject sectionToPool;
    public int amountSectionsToPool;

    [Header("Obstacles Info")]
    public GameObject[] obstaclesToPool;
    public int[] amountObstaclesToPool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        sharedInstance = this;
        pooledSections = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountSectionsToPool; i++)
        {
            tmp = Instantiate(sectionToPool);
            tmp.SetActive(false);
            pooledSections.Add(tmp);
        }

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
    public GameObject GetPooledSections()
    {
        for (int i = 0; i < amountSectionsToPool; i++)
        {
            if (!pooledSections[i].activeInHierarchy)
            {
                return pooledSections[i];
            }
        }
        return null;
    }
}
