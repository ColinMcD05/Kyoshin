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

    // Last selected object variables
    Button lastSelectedButton;

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
    public Button resume, returnButton;
    [SerializeField]
    Slider masterVolume, musicVolume;
    [SerializeField] Canvas controls;

    // Other game object and script references
    Timing timing;
    GameObject timingUI;
    GameObject winScreen;
    PlayerLevelMovement playerMovement;
    GameManager gameManager;
    [SerializeField] InputActionReference uiNavigations;
    [SerializeField] Toggle uiToggle;

    AudioSource buttonSource;
    public AudioClip buttonSound;
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
        uiNavigations.action.performed += ChangeLastSelected;
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
            // Play sound effect
            buttonSource.PlayOneShot(buttonSound);
            Pause();
        }
        else
        {
            // Play sound effect
            buttonSource.PlayOneShot(buttonSound);
            Resume();
        }
    }
    #endregion

    // Pause and Resume
    #region
    // Pauses game
    public void Pause()
    {
        lastSelectedButton = resume;
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
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
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
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        Application.Quit();
    }

    // Loads the HUB scene
    public void GoToHub()
    {
        Time.timeScale = 1;
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        SceneManager.LoadScene("HUB");
    }

    // Turn on the controls screen
    public void Controls()
    {
        controls.enabled = true;
        eventSystem.SetSelectedGameObject(returnButton.gameObject);
    }

    // Returns back to the pause menu
    public void Return()
    {
        controls.enabled = false;
        eventSystem.SetSelectedGameObject(lastSelectedButton.gameObject);
    }

    // Arrows behavior
    public void Arrows(string name)
    {
        Slider slider = null;
        switch (name)
        {
            default:
            case "Up":
                if (lastSelectedButton.navigation.selectOnUp.gameObject == null) return;
                eventSystem.SetSelectedGameObject(lastSelectedButton.navigation.selectOnUp.gameObject);
                lastSelectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
                break;
            case "Down":
                if (lastSelectedButton.navigation.selectOnDown.gameObject == null) return;
                eventSystem.SetSelectedGameObject(lastSelectedButton.navigation.selectOnDown.gameObject);
                lastSelectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
                break;
            case "Right":
                Debug.Log(eventSystem.currentSelectedGameObject == null);
                if (eventSystem.currentSelectedGameObject.CompareTag("MusicSlider") || eventSystem.currentSelectedGameObject.CompareTag("MasterSlider"))
                {
                    slider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
                    if (slider.value > slider.minValue)
                    {
                        slider.value -= 0.1f;
                        if (slider.value < slider.minValue)
                        {
                            slider.value = slider.minValue;
                        }
                    }
                }
                break;
            case "Left":
                Debug.Log(eventSystem.currentSelectedGameObject == null);
                if (eventSystem.currentSelectedGameObject.CompareTag("MusicSlider") || eventSystem.currentSelectedGameObject.CompareTag("MasterSlider"))
                {
                    slider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
                    if (slider.value < slider.maxValue)
                    {
                        slider.value += 0.1f;
                        if (slider.value < slider.maxValue)
                        {
                            slider.value = slider.maxValue;
                        }
                    }
                }
                break;
        }
    }

    public void ToggleIndicator()
    {
        uiToggle.isOn = !uiToggle.isOn;
        if (timingUI != null)
        {
            if (uiToggle.isOn)
            {
                timingUI.GetComponent<Canvas>().enabled = true;
            }
            else
            {
                timingUI.GetComponent<Canvas>().enabled = false;
            }
        }
    }

    public void Enter()
    {
        eventSystem.SetSelectedGameObject(lastSelectedButton.gameObject);
        lastSelectedButton.onClick.Invoke();
    }

    public void MoveToSlider()
    {
        lastSelectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        eventSystem.SetSelectedGameObject(lastSelectedButton.transform.GetChild(0).gameObject);
    }

    #endregion

    // GetReferences
    #region
    // Gets level references needed as pause menu is a permanent object
    void GetReferences(Scene scene, LoadSceneMode mode)
    {
        // Assigns music audio source
        music = GameObject.Find("Audio").transform.Find("Music").GetComponent<AudioSource>();
        buttonSource = GameObject.Find("Audio").transform.Find("SoundEffects").GetComponent<AudioSource>();
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

            // Set Timing Ui canvas on or off based on settings
            if (uiToggle.isOn)
            {
                timingUI.GetComponent<Canvas>().enabled = true;
            }
            else
            {
                timingUI.GetComponent<Canvas>().enabled = false;
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

    // ChangeLastSelected
    #region
    void ChangeLastSelected(InputAction.CallbackContext callbackContext)
    {
        if (!eventSystem.currentSelectedGameObject.CompareTag("Button"))
        {
            lastSelectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        }
    }
    #endregion
}
