using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int combo = 0;
    
    public void GameOver(){
        //Debug.Log("Game Over");
    }
    public void AddScore(int value){
        score += value;
        //Debug.Log("Score: " + score);
    }
}
