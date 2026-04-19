using System.Collections;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class NewSection : MonoBehaviour
{
    public GameObject nextSection;
    static GameObject lastSection;
    GameObject parent;
    SectionManager sectionManager;
    SpawnObjects spawnObjects;
    Timing timing;

    IEnumerator destroySelf;
    float waitPeriod = 6.2f;

    static int hit = 0;
    public bool destroying = false;
    public bool alreadySpawned = false;

    void Awake()
    {
        lastSection = null;
        parent = GameObject.Find("SectionManager");
        sectionManager = parent.GetComponent<SectionManager>();
        spawnObjects = sectionManager.GetComponent<SpawnObjects>();
        timing = GameObject.Find("Player").GetComponent<Timing>();
    }

    void Start()
    {

    }

    void OnEnable()
    {
        destroySelf = SelfDestroy();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (timing.currentSong.length - timing.songPosition <= 3)
            {
                hit++;
            }
            else 
            {
                if (hit != 0)
                {
                    hit = 0;
                }
                Debug.Log("Doing This");
            }
            if (hit == 4)
            {
                timing.ChangeSong();
                spawnObjects.ChangeVariables(timing.currentSong, other.transform.position);
            }
            if (!alreadySpawned)
            {
                SpawnNewSection(other);
            }
            if (!destroying)
            {
                StartCoroutine(destroySelf);
            }
            else
            {
                StopAllCoroutines();
                destroySelf = SelfDestroy();
                destroying = false;
            }
        }
    }

    void SpawnNewSection(Collider other)
    {
        Timing timing = other.GetComponent<Timing>();
        Songs.SongData currentSong = timing.currentSong;
        if (lastSection == null)
        {
            lastSection = parent.transform.GetChild(6).gameObject;
        }
        Vector3 spawnPosition = lastSection.transform.position + new Vector3(0, 0, 32);
        GameObject section = ObjectPool.sharedInstance.GetPooledSections();
        if (section != null)
        {
            section.transform.parent = parent.transform;
            section.transform.position = spawnPosition;
            section.transform.rotation = parent.transform.rotation;
            section.SetActive(true);
            lastSection = section;
        }
        alreadySpawned = true;
    }

    IEnumerator SelfDestroy()
    {
        destroying = true;
        yield return new WaitForSeconds(waitPeriod);
        if (destroying)
        {
            alreadySpawned = false;
            destroying = false;
            transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        destroying = false;
        alreadySpawned = false;
    }
}
