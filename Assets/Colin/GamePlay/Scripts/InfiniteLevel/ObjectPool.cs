using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool sharedInstance;
    public List<GameObject> pooledSections, obstaclesToPool, pooledObstacles;
    public GameObject sectionToPool;
    public int amountSectionsToPool, amountObstaclesToPool;

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
