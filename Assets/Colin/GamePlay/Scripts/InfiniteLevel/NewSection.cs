using System.Collections;
using UnityEngine;

public class NewSection : MonoBehaviour
{
    // Variables
    #region
    // Variable to hold last section spawned
    static GameObject lastSection;

    // Other references
    GameObject parent; // Parent object that sections will get assigned to
    SectionManager sectionManager;
    SpawnObjects spawnObjects;
    Timing timing;

    // Destroy self funstion holder
    IEnumerator destroySelf;

    // Other Variables
    float waitPeriod = 6.2f; // Period before object is sent back to object pool
    static float twoMeasures; // how long two measures take

    static int hit = 0; // How many colliders were hit after music ends to restart everything
    public bool destroying = false; // Whether the current object is being destroyed
    public bool alreadySpawned = false; // Whether the object has already spawned a section
    #endregion

    // Awake
    #region
    void Awake()
    {
        // Set lastsection to null and get all needed references
        lastSection = null;
        parent = GameObject.Find("SectionManager");
        sectionManager = parent.GetComponent<SectionManager>();
        spawnObjects = sectionManager.GetComponent<SpawnObjects>();
        timing = GameObject.Find("Player").GetComponent<Timing>();
    }
    #endregion

    // OnEnable
    #region
    void OnEnable()
    {
        // When enabled set destroySelf to the function SelfDestroy
        destroySelf = SelfDestroy();
    }
    #endregion

    // OnTriggerEnter
    #region
    private void OnTriggerEnter(Collider other)
    {
        // Transform variable for the next section
        Transform sectionTransform = null;
        if (other.gameObject.CompareTag("Player"))
        {
            // Check if the current section has spawned in the next area. If it hasn't continue
            if (!alreadySpawned)
            {          
                // Make the next section and spawn it in
                sectionTransform = SpawnNewSection(other);

                // if song is almost over or over, start adding to the hit variable
                if (timing.currentSong.length - timing.songPosition <= 3 || timing.songPosition <= 1)
                {
                    hit++;
                }
                else
                {
                    // If hit is not 0 and the song is not over or almost over, set hit to zero
                    if (hit != 0)
                    {
                        hit = 0;
                    }
                    // Spawn obstacles after two measures
                    if (timing.songPosition > twoMeasures)
                    {
                        spawnObjects.SpawnObject(sectionTransform);
                    }
                }
            }
            if (hit == 4)
            {
                // Once hit 4  sections after song is almost over or over, change song and reset variables based on the new song
                timing.ChangeSong();
                spawnObjects.ChangeVariables(timing.currentSong, other.transform.position);
                twoMeasures = timing.currentSong.bps * 12;
            }
            // Start destruction process if not already destroying self
            if (!destroying)
            {
                StartCoroutine(destroySelf);
            }
            // Else stop destroying object and ensuring that destroySelf is equal to the function
            else
            {
                StopAllCoroutines();
                destroySelf = SelfDestroy();
                destroying = false;
            }
        }
    }
    #endregion

    // SpawnNewSection
    #region
    Transform SpawnNewSection(Collider other)
    {
        // Get the current song
        Songs.SongData currentSong = timing.currentSong;
        // If lastSection if null, set it to the last section when they first spawn in
        if (lastSection == null)
        {
            int childNeeded = parent.transform.childCount - 1;
            lastSection = parent.transform.GetChild(childNeeded).gameObject;
        }
        // Set spawn position based on lastSection current position
        Vector3 spawnPosition = lastSection.transform.position + new Vector3(0, 0, 32);
        // Get a new section from the object pool
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

        return section.transform;
    }
    #endregion

    IEnumerator SelfDestroy()
    {
        destroying = true;
        yield return new WaitForSeconds(waitPeriod);
        if (destroying)
        {
            alreadySpawned = false;
            destroying = false;
            for (int i = 1; i < transform.parent.childCount; i++)
            {
                Transform child = transform.parent.transform.GetChild(i);
                if (child.gameObject.CompareTag("NormalObs"))
                {
                    child.gameObject.SetActive(false);
                }
            }
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
