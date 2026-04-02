using UnityEngine;


public class Obstacle : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerControllerLevel playerControllerLevel;
    public int collidedAmount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(gameManager == null){
            gameManager = FindFirstObjectByType<GameManager>();
        }
        if(playerControllerLevel == null){
            playerControllerLevel = FindFirstObjectByType<PlayerControllerLevel>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            //playerControllerLevel.ShakeCamera(10);
            collidedAmount++;
            Debug.Log("Collided Amount: " + collidedAmount);
            if(collidedAmount >= 3){
                playerControllerLevel.LoseLife();
                collidedAmount = 0;
            }
        }
    }
}
