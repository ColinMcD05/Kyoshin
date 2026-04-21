using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int combo = 0;
    public int lives = 3;

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
}
