using TMPro;
using UnityEngine;

public class Highscores : MonoBehaviour
{
    float highscore;
    GameManager gameManager;
    public Transform facingCamera;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int levelIndex = 0;
        for (int i = 0; i < gameManager.levels.Length; i++)
        {
            if (gameManager.levels[i].name == gameObject.name)
            {
                levelIndex = i;
                i = gameManager.levels.Length;
            }
        }
        highscore = gameManager.levels[levelIndex].highScore;
        gameObject.GetComponent<TextMeshPro>().text = gameObject.name + "\n Highscore: " + highscore;

        Vector3 direction = (facingCamera.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z ), Vector3.up);
        transform.rotation = targetRotation;
        transform.localRotation *= Quaternion.Euler(new Vector3(0, 180, 0));
    }
}
