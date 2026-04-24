using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private float lastToggleTime;
    private float startPauseTime;
    private float endPauseTime;
    private float toggleCooldown = 0.2f;
    private float audioValue;
    [SerializeField] InputActionReference pause;
    [SerializeField] AudioMixer audioMixer;
    AudioSource music;
    Timing timing;
    GameObject timingUI;
    GameObject winScreen;
    public EventSystem eventSystem;
    public Button resume;
    PlayerLevelMovement playerMovement;
    GameManager gameManager;
    [SerializeField] Slider masterVolume, musicVolume;

    private void Awake()
    {
        pause.action.performed += PausePerformed;
        SceneManager.sceneLoaded += GetReferences;
    }

    IEnumerator Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        while (audioMixer == null)
        {
            yield return null;
        }
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", 1)) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1)) * 20);
    }

    private void OnDestroy()
    {
        pause.action.performed -= PausePerformed; 
        SceneManager.sceneLoaded -= GetReferences;
    }

    void PausePerformed(InputAction.CallbackContext context)
    {
        if (winScreen != null && winScreen.transform.Find("WinScreen").gameObject.activeInHierarchy == false)
        {
            if (SceneManager.GetActiveScene().name == "LoseScreen") return;
            if (Time.realtimeSinceStartup - lastToggleTime < toggleCooldown) return;
            if (SceneManager.GetActiveScene().name != "HUB" && Time.timeSinceLevelLoad < 2) return;

            lastToggleTime = Time.realtimeSinceStartup;

            if (Time.timeScale != 0.0001f)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }
    public void Pause()
    {
        if (timing != null)
        {
            timing.UnSubscribeActions();
        }

        if (playerMovement != null)
        {
            playerMovement.UnSubscribeActions();
        }
        Debug.Log("Hello");
        Time.timeScale = 0.0001f;
        startPauseTime = (float)AudioSettings.dspTime;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        if (SceneManager.GetActiveScene().name != "HUB")
        {
            gameObject.transform.GetChild(0).gameObject.transform.Find("Retry").gameObject.SetActive(true);
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.transform.Find("Retry").gameObject.SetActive(false);
        }
        if (timingUI != null)
        {
            timingUI.SetActive(false);
        }
        music.Pause();
        eventSystem.firstSelectedGameObject = resume.gameObject;
        masterVolume.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 1);
    }

    public void Resume()
    {
        if (timing != null)
        {
            timing.SubscribeActions();
        }
        Time.timeScale = 1;
        endPauseTime = (float)AudioSettings.dspTime;
        if (timing != null)
        {
            timing.rewindTimeUsed += endPauseTime - startPauseTime;
        }
        if (timingUI != null)
        {
            timingUI.SetActive(true);
        }
        music.UnPause();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Retry()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameManager.transform.Find("Canvas").GetComponent<Canvas>().enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangeVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void ChangeMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GoToHub()
    {
        SceneManager.LoadScene("HUB");
    }

    void GetReferences(Scene scene, LoadSceneMode mode)
    {
        music = GameObject.Find("Audio").transform.Find("Music").GetComponent<AudioSource>();
        if (scene.name != "HUB")
        {
            timing = GameObject.Find("Player").GetComponent<Timing>(); 
            playerMovement = GameObject.Find("Player").GetComponent<PlayerLevelMovement>();
            timingUI = GameObject.Find("TimingUI");
            if (gameManager != null)
            {
                gameManager.transform.Find("Canvas").GetComponent<Canvas>().enabled = true;
            }
            if (scene.name == "Infinite")
            {
                winScreen = null;
            }
            winScreen = GameObject.Find("WinScreen");
        }
        else
        {
            timing = null;
            timingUI = null;
            winScreen = null;
        }
    }
}
