using UnityEngine;

public class SectionManager : MonoBehaviour
{
    void Start()
    {
        Vector3 spawnPosition = new Vector3(0, 0, 16);
        GameObject section = ObjectPool.sharedInstance.GetPooledObject();
        if (section != null)
        {
            section.transform.parent = gameObject.transform;
            section.transform.position = spawnPosition;
            section.transform.rotation = gameObject.transform.rotation;
            section.SetActive(true);
        }

        spawnPosition += new Vector3(0, 0, 32);
        section = ObjectPool.sharedInstance.GetPooledObject();
        if (section != null)
        {
            section.transform.parent = gameObject.transform;
            section.transform.position = spawnPosition;
            section.transform.rotation = gameObject.transform.rotation;
            section.SetActive(true);
        }

        spawnPosition += new Vector3(0, 0, 32);
        section = ObjectPool.sharedInstance.GetPooledObject();
        if (section != null)
        {
            section.transform.parent = gameObject.transform;
            section.transform.position = spawnPosition;
            section.transform.rotation = gameObject.transform.rotation;
            section.SetActive(true);
        }
    }
}
