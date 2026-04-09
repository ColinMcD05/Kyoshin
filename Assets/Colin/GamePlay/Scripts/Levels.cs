using UnityEngine;

public class Levels
{
    public string name;
    public int level;
    public float highScore;
    public Progress progress;
    public LockStatus lockStatus;

    public enum Progress
    {
        incompleted,
        completed
    }

    public enum LockStatus
    {
        Locked,
        Unlocked
    }
}
