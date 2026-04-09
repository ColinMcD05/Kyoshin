using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int combo = 0;
    public int lives = 3;

    string filePath;
    string dataPath;

    [HideInInspector] public Levels[] levels = new Levels[4];
    LevelList levelList = new LevelList();

    public void Start()
    {
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

    void Load()
    {
        dataPath = filePath + "Level_Data.json";
        if (Directory.Exists(filePath))
        {
            if (File.Exists(dataPath))
            {
                using (StreamReader stream = new StreamReader(dataPath))
                {
                    var levelString = stream.ReadToEnd();
                    var levelData = JsonUtility.FromJson<LevelList>(levelString);

                    for (int i = 0; i < levelData.levelList.Length; i++)
                    {
                        levels[i] = levelData.levelList[i];
                    }
                }
            }
        }
        else
        {
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

    public void Save()
    {
        dataPath = filePath + "Level_Data.json";
        levelList.levelList = levels;
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        string levelString = JsonUtility.ToJson(levelList, true);
        using (StreamWriter stream = File.CreateText(dataPath))
        {
            stream.WriteLine(levelString);
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
