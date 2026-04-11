using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;
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
    static GameManager instance; // instance for persistant objects

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        filePath = Application.persistentDataPath + "/Player_Data/";
        Load();
    }

    public void GameOver(){
        //Debug.Log("Game Over");
    }
    public void AddScore(int value){
        score += value;
        if (score % 100 == 0)
        {
            lives++;
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

    private void OnApplicationQuit()
    {
        // Save game once application ends
        Save();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        score = 0;
    }
}
