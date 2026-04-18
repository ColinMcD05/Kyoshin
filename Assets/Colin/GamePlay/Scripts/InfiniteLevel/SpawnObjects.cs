using UnityEngine;

public class SpawnObjects : MonoBehaviour
{

    GameObject sectionManager;
    GameObject leftSpawn;
    GameObject middleSpawn;
    GameObject rightSpawn;

    void Awake()
    {
        sectionManager = GameObject.Find("SectionManager");
        leftSpawn = sectionManager.transform.Find("OstaclesLeft").gameObject;
        middleSpawn = sectionManager.transform.Find("OstaclesMiddle").gameObject;
        rightSpawn = sectionManager.transform.Find("OstaclesRight").gameObject;
    }
}
