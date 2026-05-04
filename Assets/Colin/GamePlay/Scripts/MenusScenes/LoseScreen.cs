using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseScreen : MonoBehaviour
{
    GameManager gameManager;
    AudioSource music;
    string lastScene;
    EventSystem eventsystem;
    public Button retry;

    private void Start()
    {
        music = GameObject.Find("Audio").transform.Find("Music").GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        lastScene = gameManager.GetLastScene();
        Debug.Log(lastScene);
        music.Stop();
        music.clip = null;
        music.pitch = 1;
        Time.timeScale = 1;
        eventsystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        eventsystem.SetSelectedGameObject(retry.gameObject);
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
