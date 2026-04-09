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

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerControllerLevel>().forwardSpeed = other.GetComponent<PlayerControllerLevel>().maxSpeed;
            moveImage.enabled = false;
            stillImage.enabled = false;
            Songs.SongData currentSong = other.GetComponent<Timing>().currentSong;
            gameManager.levels[currentSong.level].progress = Levels.Progress.completed;
            if (gameManager.score > gameManager.levels[currentSong.level]
            {
                gameManager.levels[currentSong.level].highScore = gameManager.score;
            }
            if (gameManager.levels[4].lockStatus == Levels.LockStatus.Locked) 
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
                        gameManager.levels[4].lockStatus = Levels.LockStatus.Unlocked;
                    }
                }
            }
            other.GetComponent<Timing>().enabled = false;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        float alpha = 0;
        Color color = image.color;
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
