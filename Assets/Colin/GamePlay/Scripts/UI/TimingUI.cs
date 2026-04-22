using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimingUI : MonoBehaviour
{
    // References
    [SerializeField] Timing timing;
    [SerializeField] Image movingImage;
    [SerializeField] Image stillImage;

    // Variables needed
    Songs.SongData currentSong;
    float messUp;

    IEnumerator Start()
    {
        messUp = timing.messUpRange;
        currentSong = timing.currentSong;
        while (currentSong == null)
        {
            currentSong = timing.currentSong; 
            yield return null;
        }
        stillImage.transform.localScale = new Vector3(0.69f, 0.69f, 0.69f) * (currentSong.bps+0.5f);
        movingImage.transform.localScale = new Vector3(1.035f, 1.035f, 1.035f);
    }

    public IEnumerator ResetCircle()
    {
        float waitPeriod = 0;
        float checkPeriod = 1 - messUp;
        while (enabled) 
        {
            if (timing.songPositionInBeats - Mathf.Floor(timing.songPositionInBeats) <= checkPeriod)
            {
                movingImage.transform.localScale -= Vector3.one * Time.deltaTime / currentSong.bps;
                yield return null;
                //Debug.Log("Bad");
            }
            else
            {
                while (movingImage.transform.localScale.x >= stillImage.transform.localScale.x)
                {
                    movingImage.transform.localScale -= Vector3.one * Time.deltaTime / currentSong.bps;
                }
                waitPeriod = currentSong.bps * (1 - Mathf.Floor(timing.songPositionInBeats));
                //Debug.Log("Good");
                stillImage.color = Color.green;
                yield return new WaitForSeconds(waitPeriod);
                movingImage.transform.localScale = new Vector3(1.035f, 1.035f, 1.035f);
                stillImage.color = Color.red;
                //ebug.Log("Reset");
            }
        }
    }
}
