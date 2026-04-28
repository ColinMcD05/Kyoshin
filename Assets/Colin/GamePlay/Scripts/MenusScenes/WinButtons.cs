using UnityEngine;
using UnityEngine.SceneManagement;

public class WinButtons : MonoBehaviour
{

    GameManager gameManager;
    AudioSource buttonSource;
    public AudioClip buttonSound;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        buttonSource = GameObject.Find("Audio").transform.Find("SoundEffects").GetComponent<AudioSource>();
    }

    public void Retry()
    {
        // Ensures game get unpaused and pause menu is deactivated
        Time.timeScale = 1;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);

        // Enusres that game UI is active on screen
        gameManager.transform.Find("Canvas").GetComponent<Canvas>().enabled = true;
        // Reloads the game
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Loads the HUB scene
    public void GoToHub()
    {
        // Play sound effect
        buttonSource.PlayOneShot(buttonSound);
        SceneManager.LoadScene("HUB");
    }
}
