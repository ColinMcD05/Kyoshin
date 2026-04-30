using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public Image blackScreen;

    void Start()
    {
        StartCoroutine(FadingIn());
    }

    IEnumerator FadingIn()
    {
        blackScreen.enabled = true;
        Color color = blackScreen.color;
        float alpha = 1;
        while (blackScreen.color.a >= 0)
        {
            alpha -= Time.deltaTime / 4;
            color.a = alpha;
            blackScreen.color = color;
            yield return null;
        }
        blackScreen.gameObject.SetActive(false);
    }

}
