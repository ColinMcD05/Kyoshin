using UnityEngine;

public class SectionManager : MonoBehaviour
{
    public AreaType currentArea;
    public enum AreaType
    {
        AllOpen,
        LeftClosed,
        RightClosed,
        MiddleClosed,
        WallRun,
        CloseWallRun
    }
    void Start()
    {
        // Sets up the original 6 sections at the start
        Vector3 spawnPosition = new Vector3(0, 0, 16);
        GameObject section = ObjectPool.sharedInstance.GetPooledSections();
        if (section != null)
        {
            section.transform.parent = gameObject.transform;
            section.transform.position = spawnPosition;
            section.transform.rotation = gameObject.transform.rotation;
            section.SetActive(true);
        }

        spawnPosition += new Vector3(0, 0, 32);
        section = ObjectPool.sharedInstance.GetPooledSections();
        if (section != null)
        {
            section.transform.parent = gameObject.transform;
            section.transform.position = spawnPosition;
            section.transform.rotation = gameObject.transform.rotation;
            section.SetActive(true);
        }

        spawnPosition += new Vector3(0, 0, 32);
        section = ObjectPool.sharedInstance.GetPooledSections();
        if (section != null)
        {
            section.transform.parent = gameObject.transform;
            section.transform.position = spawnPosition;
            section.transform.rotation = gameObject.transform.rotation;
            section.SetActive(true);
        }

        spawnPosition += new Vector3(0, 0, 32);
        section = ObjectPool.sharedInstance.GetPooledSections();
        if (section != null)
        {
            section.transform.parent = gameObject.transform;
            section.transform.position = spawnPosition;
            section.transform.rotation = gameObject.transform.rotation;
            section.SetActive(true);
        }
    }
}
