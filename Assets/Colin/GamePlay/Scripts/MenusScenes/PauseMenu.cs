using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // Variables
    #region
    static GameObject instance;

    // Variables to check how long game has been paused
    private float startPauseTime;
    private float endPauseTime;
    bool hasStarted;

    // Last selected object variables
    Button lastSelectedButton;
    Slider lastSlider;

    // Toggle variables to avoid pausing too often in a short time
    private float lastToggleTime;
    private float toggleCooldown = 0.2f;

    // Input action references
    [SerializeField] InputActionReference pause;

    // Audio references
    [SerializeField] AudioMixer audioMixer;
    AudioSource music, playerAudio;

    // Pause menu canvas references
    public EventSystem eventSystem;
    public Button resume, returnButton, retryButton, quitButton, hubButton, toggleButton, nextButton, backButton;
    [SerializeField]
    Slider masterVolume, musicVolume;
    [SerializeField] Canvas controls;
    public Image[] controlImages;
    int currentImage = 0;

    // Other game object and script references
    Timing timing;
    GameObject indicator;
    GameObject timingUI;
    GameObject winScreen;
    PlayerLevelMovement playerMovement;
    GameManager gameManager;
    [SerializeField] InputActionReference uiNavigations;
    [SerializeField] Toggle uiToggle;
    InputSystemUIInputModule inputModule;

    AudioSource buttonSource;
    public AudioClip buttonSound;
    #endregion

    // Awake
    #region
    private void Awake()
    {
        if (instance == null)
        {
            pause.action.performed += PausePerformed; // Assigning pause action
            SceneManager.sceneLoaded += SetRetryOnButtonDown;
            SceneManager.sceneLoaded += GetReferences; // Assign Getting references once a new scene is loaded
            instance = gameObject;
        }
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
        // Get the input module to assign functions to the action for moving with keyboard
        inputModule = eventSystem.gameObject.GetComponent<InputSystemUIInputModule>();
    }
    #endregion

    // OnDestroy
    #region
    private void OnDestroy()
    {
        // Unsubscribe actions when object is destroyed
        if (instance.GetEntityId() == gameObject.GetEntityId())
        {
            pause.action.performed -= PausePerformed;
            SceneManager.sceneLoaded -= SetRetryOnButtonDown;
            SceneManager.sceneLoaded -= GetReferences;
        }
    }
    #endregion

    // PausePerformed
    #region
    // Function occurs when the pause button is pressed
    void PausePerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Hello");
        // Checks to see if winScreen is in hierachery and active. If both are trye the pause screen does not appear.
        if (winScreen != null && winScreen.transform.Find("WinScreen").gameObject.activeInHierarchy == true) return;
        // Check if the current scene is the Lose screen. If it is return and do not pause
        if (SceneManager.GetActiveScene().name == "LoseScreen") return;
        // Avoids pausing too often
        if (Time.realtimeSinceStartup - lastToggleTime < toggleCooldown) return;
        // Halt pausing a cuople of seconds to make sure nothing breaks
        if (SceneManager.GetActiveScene().name != "HUB" && Time.timeSinceLevelLoad < 3) return;

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
        inputModule.move.action.performed += ChangeLastSelected;
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
            EnableButton(retryButton);
        }
        else
        {
           DisableButton(retryButton);
        }

        // Pauses music
        music.Pause();
        if (playerAudio != null)
        {
            playerAudio.Pause();
        }

            // Sets first selected game object to the resume button
            eventSystem.SetSelectedGameObject(resume.gameObject);
        lastSelectedButton = resume;

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
        if (playerAudio != null)
        {
            playerAudio.UnPause();
        }
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        buttonSource.PlayOneShot(buttonSound);
        inputModule.move.action.performed -= ChangeLastSelected;
    }
    #endregion

    // Other pause UI functions
    #region
    
    // Restarts the current level
    public void Retry()
    {
        if (SceneManager.GetActiveScene().name == "HUB") return;
        // Ensures game get unpaused and pause menu is deactivated
        Time.timeScale = 1;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);

        // Enusres that game UI is active on screen
        gameManager.transform.Find("Canvas").GetComponent<Canvas>().enabled = true;
        // Reloads the game
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        music.clip = null;
        music.UnPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Changes master volume
    public void ChangeVolume(float value)
    {
        buttonSource.PlayOneShot(buttonSound);
        // Logarithmically change master volume based on value
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        // Saves setting in PlayerPrefs
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    // Change music volume
    public void ChangeMusicVolume(float value)
    {
        buttonSource.PlayOneShot(buttonSound);
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
        music.Stop();
        if (playerAudio != null)
        {
            playerAudio.Stop();
        }
        SceneManager.LoadScene("HUB");
    }

    // Turn on the controls screen
    public void Controls()
    {
        buttonSource.PlayOneShot(buttonSound);
        controls.enabled = true;
        eventSystem.SetSelectedGameObject(nextButton.gameObject);
    }

    // Returns back to the pause menu
    public void Return()
    {
        buttonSource.PlayOneShot(buttonSound);
        controls.enabled = false;
        eventSystem.SetSelectedGameObject(lastSelectedButton.gameObject);
    }

    public void Next()
    {
        controlImages[currentImage].enabled = false;
        currentImage++;
        if (currentImage == controlImages.Length - 1)
        {
            nextButton.gameObject.SetActive(false);
            eventSystem.SetSelectedGameObject(backButton.gameObject);
        }
        if (currentImage != 0 && !backButton.gameObject.activeInHierarchy)
        {
            backButton.gameObject.SetActive(true);
        }
        controlImages[currentImage].enabled = true;
    }

    public void Back()
    {
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
    }

    // Arrows behavior
    public void Arrows(string name)
    {
        buttonSource.PlayOneShot(buttonSound);
        Slider slider = null;
        // Gets the type of arrow to assign correct behavior
        switch (name)
        {
            default:
            case "Up":
                if (lastSlider != null)
                {
                    lastSlider = null;
                }
                // On up arrow, if button is connected up, move up
                if (lastSelectedButton.navigation.selectOnUp == null)
                {
                    eventSystem.SetSelectedGameObject(lastSelectedButton.gameObject); 
                    return;
                }
                // Set selected game object and set last selected button to current gameobject
                eventSystem.SetSelectedGameObject(lastSelectedButton.navigation.selectOnUp.gameObject);
                lastSelectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
                break;
            case "Down":
                if (lastSlider != null)
                {
                    lastSlider = null;
                }
                // On down arrow, if button is connected down, move down
                if (lastSelectedButton.navigation.selectOnDown == null)
                {
                    eventSystem.SetSelectedGameObject(lastSelectedButton.gameObject);
                    return;
                }
                // Set selected game object and set last selected button to current game object
                eventSystem.SetSelectedGameObject(lastSelectedButton.navigation.selectOnDown.gameObject);
                lastSelectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
                break;
            case "Left":
                buttonSource.PlayOneShot(buttonSound);
                // Check if current selected game object is a slider
                if (lastSlider != null)
                {
                    eventSystem.SetSelectedGameObject(lastSlider.gameObject);
                }
                else
                {
                    eventSystem.SetSelectedGameObject(lastSelectedButton.gameObject);
                    return;
                }
                if (eventSystem.currentSelectedGameObject.CompareTag("MusicSlider") || eventSystem.currentSelectedGameObject.CompareTag("MasterSlider"))
                {
                    Debug.Log(eventSystem.currentSelectedGameObject == null);
                    // Change slider value down
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
            case "Right":
                buttonSource.PlayOneShot(buttonSound);
                // Check if current selected game object is a slider
                if (lastSlider != null)
                {
                    eventSystem.SetSelectedGameObject(lastSlider.gameObject);
                }
                else
                {
                    eventSystem.SetSelectedGameObject(lastSelectedButton.gameObject);
                    return;
                }
                if (eventSystem.currentSelectedGameObject.CompareTag("MusicSlider") || eventSystem.currentSelectedGameObject.CompareTag("MasterSlider"))
                {
                    // Check slider value down
                    slider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
                    if (slider.value < slider.maxValue)
                    {
                        slider.value += 0.1f;
                        if (slider.value > slider.maxValue)
                        {
                            slider.value = slider.maxValue;
                        }
                    }
                }
                break;
        }
    }

    // Toggle the ui canvas on and off
    public void ToggleIndicator()
    {
        buttonSource.PlayOneShot(buttonSound);
        // switch toggle is on state
        uiToggle.isOn = !uiToggle.isOn;
        // if timing ui is available, turn it off
        if (timingUI != null)
        {
            if (uiToggle.isOn)
            {
                timingUI.GetComponent<Canvas>().enabled = true;
                if (indicator != null)
                {
                    indicator.SetActive(true);
                }
            }
            else
            {
                timingUI.GetComponent<Canvas>().enabled = false;
                if (indicator != null)
                {
                    indicator.SetActive(false);
                }
            }
        }
    }

    // Enter button for Ipod 
    public void Enter()
    {
        buttonSource.PlayOneShot(buttonSound);
        //ensure selected object is last selected
        eventSystem.SetSelectedGameObject(lastSelectedButton.gameObject);
        // Invoke last selected buttons on click event
        lastSelectedButton.onClick.Invoke();
    }

    // Moves to slider
    public void MoveToSlider()
    {
        buttonSource.PlayOneShot(buttonSound);
        // Set last selectedButton to the current button
        lastSelectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        // move current slected objecct to the slider
        eventSystem.SetSelectedGameObject(lastSelectedButton.transform.GetChild(0).gameObject);
        lastSlider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
        Debug.Log(eventSystem.currentSelectedGameObject.name);
    }

    #endregion

    // SetRetryButtonOnDown
    #region
    // Set the retry button on down stuff based on scene
    void SetRetryOnButtonDown(Scene scene, LoadSceneMode mode)
    {
        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;

        if (scene.name == "HUB")
        {
            newNav.selectOnUp = toggleButton.navigation.selectOnUp;
            EnableButton(quitButton);
            DisableButton(hubButton);
            newNav.selectOnDown = quitButton;
            toggleButton.navigation = newNav;
        }
        else
        {
            newNav.selectOnUp = toggleButton.navigation.selectOnUp;
            newNav.selectOnDown = retryButton;
            toggleButton.navigation = newNav;

            newNav.selectOnUp = retryButton.navigation.selectOnUp;
            EnableButton(hubButton);
            DisableButton(quitButton);
            newNav.selectOnDown = hubButton; 
            retryButton.navigation = newNav;
        }
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
                indicator = null;
            }
            else
            {
                winScreen = GameObject.Find("WinScreen");
                indicator = GameObject.Find("Indicators");
            }


            // Set Timing Ui canvas on or off based on settings
            if (uiToggle.isOn)
            {
                timingUI.GetComponent<Canvas>().enabled = true;
                if (indicator != null)
                {
                    indicator.SetActive(true);
                }
            }
            else
            {
                timingUI.GetComponent<Canvas>().enabled = false;
                if (indicator != null)
                {
                    indicator.SetActive(false);
                }
            }
            if (playerMovement != null)
            {
                playerAudio = playerMovement.transform.Find("RunningAudio").GetComponent<AudioSource>();
            }
        }
        else
        {
            // Set all player level references to null
            timing = null;
            timingUI = null;
            winScreen = null;
        }
        if (transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(false);
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

    // EnableButton
    #region
    void EnableButton(Button button)
    {
        button.enabled = true;
        button.GetComponent<Image>().enabled = true;
    }
    #endregion

    // DisableButton
    #region
    void DisableButton(Button button)
    {
        button.enabled = false;
        button.GetComponent<Image>().enabled = false;
    }
    #endregion
}