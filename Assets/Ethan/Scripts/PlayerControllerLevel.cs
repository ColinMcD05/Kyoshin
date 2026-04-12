using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
public class PlayerControllerLevel : MonoBehaviour
{
    [SerializeField] private Rewind rewind;
    GameManager gameManager;
    PlayerLevelMovement playerLevelMovement;
   
// Collision Variables | This is the amount of times the player has collided with an obstacle
    private int collidedAmout = 0;
    public int maxCollisions = 4;
    public float regenTime = 2.0f;

    void Awake(){
        playerLevelMovement = GetComponent<PlayerLevelMovement>();
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    

    #region Lose Life | This is a function that is called to lose a life
    // lose a life function will only be called after player collides with an obstacle x amount of times each collison will cause the camera to shake
    public void LoseLife(){
        if (Time.timeSinceLevelLoad >= 4)
        {
            if (collidedAmout == 0)
            {
                StartCoroutine(RegainLives());
            }
            collidedAmout++; // Increment the collided amount
            Debug.Log("Collided Amount: " + collidedAmout); // Log the collided amount
            if (collidedAmout >= maxCollisions)
            { // If the collided amount is greater than or equal to the max collisions
                //Debug.Log("Lives: " + lives);
                collidedAmout = 0; // Reset the collided amount
                                   // call rewind time function
                if (rewind != null)
                {
                    // disable player collider
                    rewind.StartRewind();
                }

            }
            if (gameManager.lives <= 0)
            {
                GameOver();
            }
        }
    }

    IEnumerator RegainLives()
    {
        while (collidedAmout > 0)
        { 
            yield return new WaitForSeconds(regenTime);
            collidedAmout--;
            Debug.Log("Regained");
        }
    }

    public void ShakeCamera(int shakeIntensity){
        // shake the camera by using the CinemachineShake script
        CineMachineShake.Instance.ShakeCamera(shakeIntensity);
    } // end of ShakeCamera function
    #endregion
    public void GameOver(){
        SceneManager.LoadScene("LoseScreen");
    }
}
