using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{
    // Variables
    #region
    // Important player variables
    public int score = 0;
    public int combo = 0;
    public int lives = 3;

    // Variables to get correct file pathing
    string filePath; // Path to directory
    string dataPath; // Path to specific file

    // Variables needed for saving and loading
    [HideInInspector] public Levels[] levels = new Levels[4]; // An array holding levels data
    LevelList levelList = new LevelList(); // A class holding a list of the levels for saving

    // Making gamemanager permanent
    [Header("Persistant Objects")]
    static GameManager instance; // instance for persistant objects
    [SerializeField] GameObject[] persistantObjects;

    // Canvas objects
    public TextMeshProUGUI scoreText, rewindText, comboText, multText;
    public RewindTracker rewindTracker;
    public GameObject dashSlider;
    public RawImage visualizer;
    [SerializeField] Color[] colors;

    // Variable holding last scene
    static public string lastScene;

    // Multiplier variables
    int mult;
    public int maxMult = 10;
    public int comboNeededMult = 5;
    #endregion

    // Awake
    #region
    public void Awake()
    {
        // If instance is not null destroy any duplicates
        if (instance != null)
        {
            CleanAndDestroy();
            return;
        }
        // Set game objects to not destroy on load
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            MarkObjects();
            // Subscribes actions
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnLoaded;
        }
        // Sets file path for saving and loading
        filePath = Application.persistentDataPath + "/Player_Data/";
        // Load level data
        Load();
    }
    #endregion

    // GameOver
    #region
    // When lost all lives, lood the losing screen
    public void GameOver()
    {
        SceneManager.LoadScene("LoseScreen");
    }
    #endregion

    // AddScore
    #region
    // Adds score to game manager
    public void AddScore(int value){
        // multipliers score by multiplier
        score += value * mult;
        // Change text
        scoreText.text = "Score: " + score;
        if (score % 100 < (score - value) % 100)
        {
            lives++;
            rewindTracker.ChangeText();
        }
        //Debug.Log("Score: " + score);
    }
    #endregion

    // Load
    #region
    void Load()
    {
        dataPath = filePath + "Level_Data.json"; // Set dataPath to where Level_Data is held
        // If directory and file exists, set level list to the level data saved
        if (Directory.Exists(filePath))
        {
            if (File.Exists(dataPath))
            {
                using (StreamReader stream = new StreamReader(dataPath))
                {
                    var levelString = stream.ReadToEnd(); // Reads data
                    var levelData = JsonUtility.FromJson<LevelList>(levelString); // sets data into lists to distrubute

                    for (int i = 0; i < levelData.levelList.Length; i++)
                    {
                        levels[i] = levelData.levelList[i]; // correctly assigns data to the correct list
                        if (levels[i].name == "Unlimited")
                        {
                            levels[i].name = "Infinite";
                        }
                    }
                }
            }
        }
        else
        {
            // If this is the first time opening the game set up levels information
            levels = new Levels[4]
            {
                new Levels
                {
                    name = "Hakone",
                    level = 1,
                    highScore = 0,
                    progress = Levels.Progress.incompleted,
                    lockStatus = Levels.LockStatus.Unlocked
                },

                new Levels
                {
                    name = "Kyoto",
                    level = 2,
                    highScore = 0,
                    progress = Levels.Progress.incompleted,
                    lockStatus = Levels.LockStatus.Unlocked
                },

                new Levels
                {
                    name = "Tokyo",
                    level = 3,
                    highScore = 0,
                    progress = Levels.Progress.incompleted,
                    lockStatus = Levels.LockStatus.Unlocked
                },

                new Levels
                {
                    name = "Infinite",
                    level = 4,
                    highScore = 0,
                    progress = Levels.Progress.incompleted,
                    lockStatus = Levels.LockStatus.Locked
                }
            };
        }
    }
    #endregion

    // Save
    #region
    public void Save()
    {
        dataPath = filePath + "Level_Data.json"; // set dataPath to where levels data is held
        levelList.levelList = levels; // Set the levelList list in the class levelList to the list levels in order to save properly
        // Checks if the directory exists
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        // Overwrite level data with current level data
        string levelString = JsonUtility.ToJson(levelList, true);
        using (StreamWriter stream = File.CreateText(dataPath))
        {
            stream.WriteLine(levelString);
        }
    }
    #endregion

    // Creating persistant object
    #region
    // Mark objects to not destroy on load
    private void MarkObjects()
    {
        foreach (GameObject obj in persistantObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    // Destroy persistent objects or duplicate objects
    private void CleanAndDestroy()
    {
        foreach (GameObject obj in persistantObjects)
        {
            Destroy(obj);
        }
        Destroy(gameObject);
    }
    #endregion

    // ApplicationQuit
    #region
    // save game and unsubscribe actions when application quits
    private void OnApplicationQuit()
    {
        // Save game once application ends
        Save();
        SceneManager.sceneLoaded -= SceneLoaded;
    }
    #endregion

    // Scene loaded scripts
    #region
    // Script that runs when scene loads
    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Saves the game to ensure no data loss
        Save();

        // set score and lives back to normal
        score = 0;
        lives = 3;

        // if title screen, get rid of game manager and all other persistent objects
        if (scene.name == "TitleScreen")
        {
            instance = null;
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneUnloaded -= SceneUnLoaded;
            CleanAndDestroy();
            scoreText.enabled = false;
            rewindText.enabled = false;
            comboText.enabled = false;
            multText.enabled = false;
            for (int i = 0; i < dashSlider.transform.childCount; i++)
            {
                dashSlider.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        // if on lose screen or hub, set UI to disabled
        else if (scene.name == "LoseScreen" || scene.name == "HUB")
        {
            scoreText.enabled = false;
            rewindText.enabled = false;
            comboText.enabled = false;
            multText.enabled = false;
            for (int i = 0; i < dashSlider.transform.childCount; i++)
            {
                dashSlider.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        // set UI to active in levels
        else
        {
            scoreText.enabled = true;
            scoreText.text = "Score: " + score;
            rewindTracker.ChangeText();
            rewindText.enabled = true;
            comboText.enabled = true;
            multText.enabled = true;
            ClearCombo();
            for (int i = 0; i < dashSlider.transform.childCount; i++)
            {
                dashSlider.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    void SceneUnLoaded(Scene scene)
    {
        // Set last scene variables
        lastScene = scene.name;
        GameObject.Find("Audio").transform.Find("SoundEffects").GetComponent<AudioSource>().Stop();
    }
    #endregion

    // Combo scripts
    #region
    // Increases combo
    public void IncreaseCombo()
    {
        // adds combo
        combo++;
        //changes mult based on combo
        mult = mult = combo / comboNeededMult + 1;
        // Ensures mult is not higher than maxMult
        if (mult > maxMult)
        {
            mult = maxMult;
        }
        // Change combo and mult text and color
        comboText.text = combo + "X";
        comboText.color = colors[mult - 1];
        multText.text = "Mult " + mult;
        multText.color = colors[mult - 1];
    }

    public void ClearCombo()
    {
        // Reset mult and combo
        mult = 1;
        combo = 0;
        comboText.text = combo + "X";
        multText.text = "Mult " + mult;
        comboText.color = colors[0];
        multText.color = colors[0];
    }
    #endregion

    // Get data
    #region
    // Sends out the last scene variable
    public string GetLastScene()
    {
        return lastScene;
    }

    // Returns level based on scene name
    public Levels GetLevel(string name)
    {
        foreach (Levels level in levels)
        {
            if (name == level.name)
            {
                return level;
            }
        }
        return null;
    }

    // returns highscore of level based on scene name
    public float GetHighScore(string name)
    {
        foreach (Levels level in levels)
        {
            if (name == level.name)
            {
                return level.highScore;
            }
        }
        return 0;
    }
    #endregion
}
