using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int lives = 3;
    public int score = 0;
    [SerializeField] private Rewind rewind;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rewind == null)
        {
            rewind = FindFirstObjectByType<Rewind>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GameOver(){
        Debug.Log("Game Over");
    }
    public void AddScore(int value){
        score += value;
        Debug.Log("Score: " + score);
    }
}
