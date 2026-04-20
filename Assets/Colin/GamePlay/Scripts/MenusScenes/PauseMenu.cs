using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        pause.action.performed += PausePerformed;
        SceneManager.sceneLoaded += GetReferences;
    }

    IEnumerator Start()
    {
        while (audioMixer == null)
        {
            audioMixer.SetFloat("Volume", Mathf.Log10(PlayerPrefs.GetFloat("Volume", 1)) * 20);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        pause.action.performed -= PausePerformed; 
        SceneManager.sceneLoaded -= GetReferences;
    }

    void PausePerformed(InputAction.CallbackContext context)
    {
        if (Time.realtimeSinceStartup - lastToggleTime < toggleCooldown) return;

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
    public void Pause()
    {

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
    }

    public void Resume()
    {
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangeVolume(float value)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("Volume", value);
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
        if (scene.name != "Hub")
        {
            timing = GameObject.Find("Player").GetComponent<Timing>();
            timingUI = GameObject.Find("TimingUI");
        }
        else
        {
            timing = null;
            timingUI = null;
        }
    }
}
