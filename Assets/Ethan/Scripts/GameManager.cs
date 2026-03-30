using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int lives = 3;
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
        Debug.Log("Lives: " + lives);
        if(lives <= 0){
            GameOver();
        }
    }
    public void GameOver(){
        Debug.Log("Game Over");
    }
}
