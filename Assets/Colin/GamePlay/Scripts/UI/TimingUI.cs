using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimingUI : MonoBehaviour
{
    // References
    [SerializeField] Timing timing;
    [SerializeField] Image movingImage;
    [SerializeField] Image stillImage;
    public Sprite[] sprites;
    public float lastBeat = -1;

    // Variables needed
    Songs.SongData currentSong;
    float messUp;
    public float sizeMultiplier = 1.2f;
    Vector3 movingImageEnlarged;
    Color greyedOut = new Color(118f / 255, 118f/ 255, 118f/255);

    IEnumerator Start()
    {
        messUp = timing.messUpRange;
        currentSong = timing.currentSong;
        while (currentSong == null)
        {
            currentSong = timing.currentSong; 
            yield return null;
        }
        //stillImage.transform.localScale = new Vector3(0.69f, 0.69f, 0.69f) * (currentSong.bps+0.5f);
        //movingImageEnlarged = stillImage.transform.localScale + (Vector3.one * 0.5f);
        //movingImage.transform.localScale = new Vector3(1.035f, 1.035f, 1.035f);
    }

    /*public IEnumerator ResetCircle()
    {
        float waitPeriod = 0;
        float checkPeriod = 1 - messUp;
        while (enabled) 
        {
            float positionDecimal = timing.songPositionInBeats - Mathf.Floor(timing.songPositionInBeats);
            if (positionDecimal <= checkPeriod)
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
                waitPeriod = currentSong.bps * (1 - positionDecimal);
                Debug.Log("Good");
                stillImage.color = Color.green;
                yield return new WaitForSeconds(waitPeriod);
                movingImage.transform.localScale = movingImageEnlarged;
                stillImage.color = Color.red;
                Debug.Log("Reset");
            }
        }
    }*/

    public IEnumerator IndicateBeat()
    {
        float waitPeriod = 0;
        float checkPeriod = 1 - messUp;
        while (enabled)
        {
            float floorBeat = Mathf.Floor(timing.songPositionInBeats);
            float beat = timing.songPositionInBeats;
            float positionDecimal = beat - floorBeat;
            if (positionDecimal > checkPeriod && floorBeat > lastBeat)
            {
                waitPeriod = currentSong.bps * (1 - positionDecimal);
                stillImage.color = Color.white;
                yield return new WaitForSeconds(waitPeriod);
                Debug.Log("Yes");
                stillImage.sprite = ChangeSprite();
                stillImage.transform.localScale *= sizeMultiplier;
                yield return new WaitForSeconds(messUp * timing.currentSong.bps);
                stillImage.color = greyedOut;
                stillImage.transform.localScale /= sizeMultiplier;
                lastBeat = beat;
            }
            else
            {
                yield return null;
            }
        }
    }

    Sprite ChangeSprite()
    {
        foreach (Sprite sprite in sprites)
        {
            if (stillImage.sprite != sprite)
            {
                return sprite;
            }
        }
        return null;
    }
}
