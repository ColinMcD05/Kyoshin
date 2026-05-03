using System.Collections;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public int fadeOutTime = 3;
    public int haltTime = 3;
    CanvasGroup canvas;

    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        StartCoroutine(FadingOut());
    }

    IEnumerator FadingOut()
    {
        yield return new WaitForSeconds(haltTime);
        while (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime / fadeOutTime;
            yield return null;
        }
    }
}