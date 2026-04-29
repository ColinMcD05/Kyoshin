using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button playButton, returnButton;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] Image blackScreen;
    [SerializeField] Canvas howToPlayCanvas, titleScreen;

    public AudioSource buttonSource;
    public AudioClip buttonSound;

    public float fadeOutTime;

    private void Start()
    {
        eventSystem.firstSelectedGameObject = playButton.gameObject;
        StartCoroutine(FadeIn());
    }

    public void Play()
    {
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        StartCoroutine(FadeOut());
    }

    public void Quit()
    {
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        Application.Quit();
    }

    public void HowToPlay()
    {
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        howToPlayCanvas.enabled = true;
        titleScreen.enabled = false;
        eventSystem.SetSelectedGameObject(returnButton.gameObject);
    }

    public void Return()
    {
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        howToPlayCanvas.enabled = false;
        titleScreen.enabled = true;
        eventSystem.SetSelectedGameObject(playButton.gameObject);
    }

    IEnumerator FadeIn()
    {
        blackScreen.enabled = true;
        Color color = blackScreen.color;
        float alpha = 1;
        while (blackScreen.color.a >= 0)
        {
            alpha -= Time.deltaTime / 2;
            color.a = alpha;
            blackScreen.color = color;
            yield return null;
        }
        blackScreen.enabled = false;
    }

    IEnumerator FadeOut()
    {
        blackScreen.enabled = true;
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
