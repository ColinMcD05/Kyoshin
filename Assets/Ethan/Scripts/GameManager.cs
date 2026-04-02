using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int lives = 3;
    public int score = 0;
    public int combo = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoseLife(){
        lives--;
        //Debug.Log("Lives: " + lives);
        if(lives <= 0){
            GameOver();
        }
    }
    public void GameOver(){
        //Debug.Log("Game Over");
    }
    public void AddScore(int value){
        score += value;
        //Debug.Log("Score: " + score);
    }
}
