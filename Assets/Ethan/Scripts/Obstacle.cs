using UnityEngine;


public class Obstacle : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerControllerLevel playerControllerLevel;
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
            playerControllerLevel.ShakeCamera(10); // Shake the camera by 10 units
            playerControllerLevel.LoseLife();
        }
    } // end of OnTriggerEnter function
}
