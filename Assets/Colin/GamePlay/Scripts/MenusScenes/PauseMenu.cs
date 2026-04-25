using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // Variables
    #region
    // Variables to check how long game has been paused
    private float startPauseTime;
    private float endPauseTime;
    bool hasStarted;

    // Toggle variables to avoid pausing too often in a short time
    private float lastToggleTime;
    private float toggleCooldown = 0.2f;

    // Input action references
    [SerializeField] InputActionReference pause, pauseHub;

    // Audio references
    [SerializeField] AudioMixer audioMixer;
    AudioSource music;

    // Pause menu canvas references
    public EventSystem eventSystem;
    public Button resume;
    [SerializeField]
    Slider masterVolume, musicVolume;

    // Other game object and script references
    Timing timing;
    GameObject timingUI;
    GameObject winScreen;
    PlayerLevelMovement playerMovement;
    GameManager gameManager;
    #endregion

    // Awake
    #region
    private void Awake()
    {
        pause.action.performed += PausePerformed; // Assigning pause action
        SceneManager.sceneLoaded += GetReferences; // Assign Getting references once a new scene is loaded
    }
    #endregion

    // Start
    #region
    IEnumerator Start()
    {
        // Getting important, unchanging references
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // ensures audioMixer is not considered null.
        while (audioMixer == null)
        {
            yield return null;
        }
        // Sets the volume based on saved player settings
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", 1)) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1)) * 20);
    }
    #endregion

    // OnDestroy
    #region
    private void OnDestroy()
    {
        // Unsubscribe actions when object is destroyed
        pause.action.performed -= PausePerformed; 
        SceneManager.sceneLoaded -= GetReferences;
    }
    #endregion

    // PausePerformed
    #region
    // Function occurs when the pause button is pressed
    void PausePerformed(InputAction.CallbackContext context)
    {
        // Checks to see if winScreen is in hierachery and active. If both are trye the pause screen does not appear.
        if (winScreen != null && winScreen.transform.Find("WinScreen").gameObject.activeInHierarchy == true) return;
        // Check if the current scene is the Lose screen. If it is return and do not pause
        if (SceneManager.GetActiveScene().name == "LoseScreen") return;
        // Avoids pausing too often
        if (Time.realtimeSinceStartup - lastToggleTime < toggleCooldown) return;
        // Halt pausing a cuople of seconds to make sure nothing breaks
        if (SceneManager.GetActiveScene().name != "HUB" && Time.timeSinceLevelLoad < 2) return;

        // Logs the toggle time
        lastToggleTime = Time.realtimeSinceStartup;

        // Pause and unpause based on timeScale
        if (Time.timeScale != 0.0001f)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
    #endregion

    // Pause and Resume
    #region
    // Pauses game
    public void Pause()
    {
        // Unsubscribes actions on player if in a level
        if (timing != null)
        {
            if(timing.songPositionInBeats > 16)
            {
                hasStarted = true;
            }
            else
            {
                hasStarted = false;
            }
            timing.UnSubscribeActions();
        }

        if (playerMovement != null)
        {
            playerMovement.UnSubscribeActions();
        }

        // Turn off timing UI if in level
        if (timingUI != null)
        {
            timingUI.SetActive(false);
        }

        // pauses game
        Time.timeScale = 0.0001f;

        // Set pause start time
        startPauseTime = (float)AudioSettings.dspTime;

        // Truns on pause menu UI
        gameObject.transform.GetChild(0).gameObject.SetActive(true);

        // If in the HUB, do not show Retry or Hub buttons. Else, do show buttons
        if (SceneManager.GetActiveScene().name != "HUB")
        {
            gameObject.transform.GetChild(0).gameObject.transform.Find("Retry").gameObject.SetActive(true);
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.transform.Find("Retry").gameObject.SetActive(false);
        }

        // Pauses music
        music.Pause();
        
        // Sets first selected game object to the resume button
        eventSystem.firstSelectedGameObject = resume.gameObject;

        // sets the volume slider values to the values in PlayerPrefs
        masterVolume.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 1);
    }

    // Resumes game
    public void Resume()
    {
        // Resubscribes actions and turns timing UI back on
        if (timing != null)
        {
            Debug.Log(hasStarted);
            if (hasStarted)
            {
                timing.SubscribeActions();
            }
            else
            {
                playerMovement.SubscribeActions();
            }
        }
        if (timingUI != null)
        {
            timingUI.SetActive(true);
        }

        // Puts Time.timeScale to 1
        Time.timeScale = 1;
        // Sets the end time when paused
        endPauseTime = (float)AudioSettings.dspTime;
        // If in a level with timing script, add amount of rewindTimeUsed based on endPauseTime and startPauseTime
        if (timing != null)
        {
            timing.rewindTimeUsed += endPauseTime - startPauseTime;
        }
        // Unpauses music and deactivates pause menu images
        music.UnPause();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion

    // Other pause UI functions
    #region
    
    // Restarts the current level
    public void Retry()
    { 
        // Ensures game get unpaused and pause menu is deactivated
        Time.timeScale = 1;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);

        // Enusres that game UI is active on screen
        gameManager.transform.Find("Canvas").GetComponent<Canvas>().enabled = true;
        // Reloads the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Changes master volume
    public void ChangeVolume(float value)
    {
        // Logarithmically change master volume based on value
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        // Saves setting in PlayerPrefs
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    // Change music volume
    public void ChangeMusicVolume(float value)
    {
        // Logarithmically change music volume based on value
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        // Saves setting in PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    // Quits game
    public void Quit()
    {
        Application.Quit();
    }

    // Loads the HUB scene
    public void GoToHub()
    {
        SceneManager.LoadScene("HUB");
    }
    #endregion

    // GetReferences
    #region
    // Gets level references needed as pause menu is a permanent object
    void GetReferences(Scene scene, LoadSceneMode mode)
    {
        // Assigns music audio source
        music = GameObject.Find("Audio").transform.Find("Music").GetComponent<AudioSource>();
        // assigns variables if scene is not HUB
        if (scene.name != "HUB" && scene.name != "LoseScreen")
        {
            timing = GameObject.Find("Player").GetComponent<Timing>(); 
            playerMovement = GameObject.Find("Player").GetComponent<PlayerLevelMovement>();
            timingUI = GameObject.Find("TimingUI");
            if (gameManager != null)
            {
                gameManager.transform.Find("Canvas").GetComponent<Canvas>().enabled = true;
            }
            // set winscreen to null if scene name is infinite
            if (scene.name == "Infinite")
            {
                winScreen = null;
            }
            else
            {
                winScreen = GameObject.Find("WinScreen");
            }
            
        }
        else
        {
            // Set all player level references to null
            timing = null;
            timingUI = null;
            winScreen = null;
        }
    }
    #endregion
}
