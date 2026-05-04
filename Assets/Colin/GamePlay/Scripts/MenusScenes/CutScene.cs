using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{
    public CanvasGroup canvas;
    public string nextLevel;

    int cutSceneLength;
    void Start()
    {
        StartCoroutine(FadeIn());
        cutSceneLength -= 2;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        while (canvas.alpha < 1)
        {
            canvas.alpha += Time.deltaTime / 2;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(cutSceneLength);
        while (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime / 2;
            yield return null;
        }
        NextLevel();
    }

    void NextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }
}
