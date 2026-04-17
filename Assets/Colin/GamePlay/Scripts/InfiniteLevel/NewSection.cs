using System.Collections;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class NewSection : MonoBehaviour
{
    public GameObject nextSection;
    static GameObject lastSection;
    GameObject parent;

    IEnumerator destroySelf;
    float waitPeriod = 6.2f;

    public bool destroying = false;
    public bool alreadySpawned = false;

    void Awake()
    {
        lastSection = null;
        parent = GameObject.Find("SectionManager");
    }

    void OnEnable()
    {
        destroySelf = SelfDestroy();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!alreadySpawned)
            {
                SpawnNewSection(other);
            }
            Debug.Log(destroying);
            if (!destroying)
            {
                StartCoroutine(destroySelf);
            }
            else
            {
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
            lastSection = parent.transform.GetChild(2).gameObject;
        }
        Vector3 spawnPosition = lastSection.transform.position + new Vector3(0, 0, 32);
        GameObject section = ObjectPool.sharedInstance.GetPooledObject();
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
