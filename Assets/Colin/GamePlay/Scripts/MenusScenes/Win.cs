using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Win : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] Image moveImage;
    [SerializeField] Image stillImage;
    [SerializeField] Image image;
    public float fadeOutTime;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Set player speed to max speed
            other.GetComponent<PlayerLevelMovement>().forwardSpeed = other.GetComponent<PlayerLevelMovement>().maxSpeed;

            // Disable timing based mechanics
            other.GetComponent<Timing>().enabled = false;
            moveImage.enabled = false;
            stillImage.enabled = false;
            // Get current song
            Songs.SongData currentSong = other.GetComponent<Timing>().currentSong;
            // Get current level
            Levels currentLevel = gameManager.levels[currentSong.level - 1];

            // Set current levels progress to completed
            currentLevel.progress = Levels.Progress.completed;

            // If score is higher than level highscore, set highscore to score
            if (gameManager.score > currentLevel.highScore)
            {
                currentLevel.highScore = gameManager.score;
            }
            // If Unlimited mode is lcoked, checked if all three other levels have been completed and unlock it.
            if (gameManager.levels[3].lockStatus == Levels.LockStatus.Locked) 
            {
                int levelsCompleted = 0;
                for (int i = 0; i < gameManager.levels.Length - 1; i++)
                {
                    if (gameManager.levels[i].progress == Levels.Progress.completed)
                    {
                        levelsCompleted++;
                    }
                    if (levelsCompleted >= 3)
                    {
                        gameManager.levels[3].lockStatus = Levels.LockStatus.Unlocked;
                    }
                }
            }
            // Start fadeout
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        // Variables for the color to change
        float alpha = 0;
        Color color = image.color;
        // While alpha is less than 1, slowly increase alpha
        while (image.color.a <= 1)
        {
            alpha += Time.deltaTime / fadeOutTime;
            color.a = alpha;
            image.color = color;
            yield return null;
        }
        SceneManager.LoadScene("WinScene");
    }
}
