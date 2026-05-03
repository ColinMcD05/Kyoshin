using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    public int fadeOutTime = 3;
    public int fadeInTime = 3;
    public int haltTime = 3;
    CanvasGroup canvas;

    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        if (SceneManager.GetActiveScene().name != "Infinite")
        {
            StartCoroutine(FadingOut());
        }
    }

    public IEnumerator FadingOut()
    {
        yield return new WaitForSeconds(haltTime);
        while (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime / fadeOutTime;
            yield return null;
        }
    }

    public IEnumerator FadingIn()
    {
        while (canvas.alpha < 1)
        {
            canvas.alpha += Time.deltaTime / fadeOutTime;
            yield return null;
        }
        StartCoroutine(FadingOut());
    }
}