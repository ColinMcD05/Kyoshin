using TMPro;
using UnityEngine;

public class Highscores : MonoBehaviour
{
    // Variables
    GameManager gameManager;
    public Transform facingCamera; 
    float highscore;

    void Start()
    {
        // Get game manager reference
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int levelIndex = 0; // Level index attached to the highscore
        for (int i = 0; i < gameManager.levels.Length; i++)
        {
            if (gameManager.levels[i].name == gameObject.name)
            {
                // Once getting correct level to reference, set index and break out of lok
                levelIndex = i;
                break;
            }
        }
        // Set high score of level based on saves and write text
        highscore = gameManager.levels[levelIndex].highScore;
        gameObject.GetComponent<TextMeshPro>().text = gameObject.name + "\n Highscore: " + highscore;

        // Have the highscore lists face the camera
        Vector3 direction = (facingCamera.position - transform.position).normalized; // Get direction
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z ), Vector3.up);
        transform.rotation = targetRotation;
        transform.localRotation *= Quaternion.Euler(new Vector3(0, 180, 0));
    }
}
