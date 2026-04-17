using System.Collections;
using UnityEngine;

public class NewSection : MonoBehaviour
{
    public GameObject nextSection;

    IEnumerator destroySelf;
    int waitPeriod = 8;

    bool destroying = false;

    void Awake()
    {
        destroySelf = SelfDestroy();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Timing timing = other.GetComponent<Timing>();
            Songs.SongData currentSong = timing.currentSong;
            if (timing.songPosition - currentSong.length > 4)
            {
                // Do not create new parts go through two more gates on second gate choose new song and start building based on next gate position, on third gate start music
            }
            Instantiate(nextSection, new Vector3(0, 0, 80), Quaternion.Euler(0, -90, 0));
            if (!destroying)
            {
                StartCoroutine(destroySelf);
                destroying = true;
            }
            else
            {
                StopCoroutine(destroySelf);
                destroying = false;
            }
        }
    }
    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(waitPeriod);
        if (destroying)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
