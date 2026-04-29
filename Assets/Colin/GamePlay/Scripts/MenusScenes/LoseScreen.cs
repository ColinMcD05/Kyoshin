using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreen : MonoBehaviour
{
    GameManager gameManager;
    AudioSource music;
    string lastScene;

    private void Start()
    {
        music = GameObject.Find("Audio").transform.Find("Music").GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        lastScene = gameManager.GetLastScene();
        Debug.Log(lastScene);
        music.Stop();
    }

    public void Retry()
    {
        SceneManager.LoadScene(lastScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
