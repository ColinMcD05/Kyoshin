using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] Image blackScreen;
    [SerializeField] Canvas howToPlayCanvas;

    public float fadeOutTime;

    private void Start()
    {
        eventSystem.firstSelectedGameObject = playButton.gameObject;
    }

    public void Play()
    {
        // Play sound effect
        StartCoroutine(FadeOut());
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void HowToPlay()
    {
        howToPlayCanvas.enabled = true;
    }

    public void Return()
    {
        howToPlayCanvas.enabled = false;
    }

    IEnumerator FadeOut()
    {
        Color color = blackScreen.color;
        float alpha = 0;
        while (blackScreen.color.a <= 1)
        {
            alpha += Time.deltaTime / fadeOutTime;
            color.a = alpha;
            blackScreen.color = color;
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
