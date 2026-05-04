using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{
    public CanvasGroup canvas;
    public string nextLevel;

    public int cutSceneLength;

    private System.IDisposable eventListener;
    void Start()
    {
        StartCoroutine(FadeOut());
        cutSceneLength -= 2;
        StartCoroutine(FadeIn());
    }


    void OnEnable()
    {
        // Use a lambda to receive the input control, then trigger your method
        eventListener = InputSystem.onAnyButtonPress.Call(control => NextLevel());
    }

    void OnDisable()
    {
        // Always dispose your listener to prevent memory leaks or errors
        eventListener?.Dispose();
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(cutSceneLength);
        while (canvas.alpha < 1)
        {
            canvas.alpha += Time.deltaTime / 2;
            yield return null;
        }
        NextLevel();
    }

    IEnumerator FadeOut()
    {
        while (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime / 2;
            yield return null;
        }
    }

    void NextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }
}
