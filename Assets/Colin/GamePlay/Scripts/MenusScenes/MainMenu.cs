using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button playButton, returnButton, nextButton, backButton;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] Image blackScreen;
    [SerializeField] Canvas howToPlayCanvas, titleScreen;
    [SerializeField] AudioMixer audioMixer;
    public AudioSource buttonSource;
    public AudioClip buttonSound;

    public Image[] controlImages;
    int currentImage = 0;

    public float fadeOutTime;

    private void Start()
    {
        eventSystem.firstSelectedGameObject = playButton.gameObject;
        StartCoroutine(FadeIn());
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", 1)) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1)) * 20);
    }

    public void Play()
    {
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        StopAllCoroutines();
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
        currentImage = 0;
        controlImages[currentImage].enabled = true;
        backButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(nextButton.gameObject);
    }

    public void Return()
    {
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        controlImages[currentImage].enabled = false;
        howToPlayCanvas.enabled = false;
        currentImage = 0;
        backButton.gameObject.SetActive(false);
        titleScreen.enabled = true;
        eventSystem.SetSelectedGameObject(playButton.gameObject);
    }

    public void Next()
    {
        buttonSource.PlayOneShot(buttonSound);
        controlImages[currentImage].enabled = false;
        currentImage++;
        if (currentImage == controlImages.Length -1)
        {
            nextButton.gameObject.SetActive(false);
            eventSystem.SetSelectedGameObject(backButton.gameObject);
        }
        if(currentImage != 0 && !backButton.gameObject.activeInHierarchy)
        {
            backButton.gameObject.SetActive(true);
        }
        controlImages[currentImage].enabled = true;
    }

    public void Back()
    {
        buttonSource.PlayOneShot(buttonSound);
        controlImages[currentImage].enabled = false;
        currentImage--;
        if (currentImage == 0)
        {
            backButton.gameObject.SetActive(false);
            eventSystem.SetSelectedGameObject(nextButton.gameObject);
        }
        if (currentImage != controlImages.Length - 1 && !nextButton.gameObject.activeInHierarchy)
        {
            nextButton.gameObject.SetActive(true);
        }
        controlImages[currentImage].enabled = true;
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
