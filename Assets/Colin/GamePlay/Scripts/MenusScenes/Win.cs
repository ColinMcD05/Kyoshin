using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Win : MonoBehaviour
{
    // Variables
    #region
    // References
    GameManager gameManager;
    [SerializeField] MoveBackwards moveBackwards;
    [SerializeField] Image moveImage;
    [SerializeField] Image stillImage;
    [SerializeField] Image image;
    public float fadeOutTime;
    public Transform playerWinPosition, otherCharWinPosition;
    public GameObject player, otherChar;
    public Camera mainCamera;
    public Camera winCamera;
    public GameObject winScreen;
    EventSystem eventSystem;
    GameObject retry;
    AudioSource music;
    public AudioClip win;
    bool newHighScore;
    #endregion

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        otherChar = GameObject.Find("OtherChar");
        player = GameObject.Find("Player");
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        retry = winScreen.transform.Find("Retry").gameObject;
        music = GameObject.Find("Audio").transform.Find("Music").GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Set player speed to max speed
            moveBackwards.forwardSpeed = moveBackwards.maxSpeed;

            // Disable timing based mechanics
            other.GetComponent<Timing>().enabled = false;
            other.GetComponent<PlayerLevelMovement>().enabled = false;
            moveImage.enabled = false;
            stillImage.enabled = false;
            // Get current song
            Songs.SongData currentSong = other.GetComponent<Timing>().currentSong;
            // Get current level
            Levels currentLevel = gameManager.GetLevel(SceneManager.GetActiveScene().name);

            // Set current levels progress to completed
            currentLevel.progress = Levels.Progress.completed;

            music.Stop();
            music.PlayOneShot(win);

            // If score is higher than level highscore, set highscore to score
            if (gameManager.score > currentLevel.highScore)
            {
                newHighScore = true;
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
            gameManager.transform.Find("Canvas").GetComponent<Canvas>().enabled = false;
            // Start fadeout
            StartCoroutine(FadeOut(currentLevel));
        }
    }

    IEnumerator FadeOut(Levels currentLevel)
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
        //Transition();
        otherChar.GetComponent<FollowScript>().enabled = false;
        winScreen.SetActive(true);
        eventSystem.firstSelectedGameObject = retry;

        // Show score and High Score
        TextMeshProUGUI score = winScreen.transform.Find("Score").GetComponent<TextMeshProUGUI>();
        score.text = "Score: " + gameManager.score;
        TextMeshProUGUI highScore = winScreen.transform.Find("HighScore").GetComponent<TextMeshProUGUI>();
        highScore.text = "High Score: " + currentLevel.highScore;
        // if it is a new high score, create the text of a new highscore
        if (newHighScore)
        {
            // NEW HIGH SCORE
        }
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1);
        // Variables for the color to change
        float alpha = 1;
        Color color = image.color;
        // While alpha is greater than 0, slowly decrease alpha
        while (image.color.a >= 0)
        {
            alpha -= Time.deltaTime/2;
            color.a = alpha;
            image.color = color;
            yield return null;
        }
    }

    void Transition()
    {
        // Stops move backwards scripts
        moveBackwards.enabled = false;

        // Set position of both characters to win positions
        player.transform.position = playerWinPosition.position;
        otherChar.transform.position = otherCharWinPosition.position;

        // Change camera
        mainCamera.enabled = false;
        winCamera.enabled = true;

        // Spawn in whats needed

        // Play Animation, win music, show score

        StartCoroutine(FadeIn());
    }
}
