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
    public TextMeshProUGUI scoreText, rewindText, comboText, multText;
    public RewindTracker rewindTracker;
    public GameObject dashSlider;
    public RawImage visualizer;

    static public string lastScene;
    int mult;
    public int maxMult = 10;
    public int comboNeededMult = 5;

    public void Awake()
    {
        if (instance != null)
        {
            CleanAndDestroy();
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            MarkObjects();
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnLoaded;
        }
        filePath = Application.persistentDataPath + "/Player_Data/";
        Load();
    }

    public void GameOver()
    {
        SceneManager.LoadScene("LoseScreen");
    }
    public void AddScore(int value){
        score += value * mult;
        scoreText.text = "Score: " + score;
        if (score % 100 < (score - value) % 100)
        {
            lives++;
            rewindTracker.ChangeText();
        }
        //Debug.Log("Score: " + score);
    }

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
                    name = "Unlimited",
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

    private void CleanAndDestroy()
    {
        foreach (GameObject obj in persistantObjects)
        {
            Destroy(obj);
        }
        Destroy(gameObject);
    }

    private void OnApplicationQuit()
    {
        // Save game once application ends
        Save();
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Save();
        score = 0;
        lives = 3;
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

    public void IncreaseCombo()
    {
        combo++;
        mult = mult = combo / comboNeededMult + 1;
        if (mult > maxMult)
        {
            mult = maxMult;
        }
        comboText.text = combo + "X";
        multText.text = "Mult " + mult;
    }

    public void ClearCombo()
    {
        mult = 1;
        combo = 0;
        comboText.text = combo + "X";
        multText.text = "Mult " + mult;
    }

    void SceneUnLoaded(Scene scene)
    {
        lastScene = scene.name;
    }

    public string GetLastScene()
    {
        return lastScene;
    }

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
}
