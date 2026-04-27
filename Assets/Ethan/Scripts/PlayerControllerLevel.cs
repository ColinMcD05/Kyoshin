using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using TMPro;
public class PlayerControllerLevel : MonoBehaviour
{
    [SerializeField] Rewind rewind;
    [SerializeField] Timing timing;
    GameManager gameManager;
    CinemachineBasicMultiChannelPerlin cineMachineNoise;
    [SerializeField] MoveBackwards moveBackwards;
    RewindTracker livesText;

    int collidedAmout = 0;
    public int maxCollisions = 4;
    public float regenTime = 2.0f;
    public bool invincible = false;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cineMachineNoise = GameObject.Find("CinemachineCamera").GetComponent<CinemachineBasicMultiChannelPerlin>();
        livesText = gameManager.transform.Find("Canvas").transform.Find("RewindCounter").GetComponent<RewindTracker>();
    }

    private void OnEnable()
    {
        invincible = false;
    }

    void OnDisable()
    {
        invincible = true;
    }

    //Lose Life | This is a function that is called to lose a life
    #region
    // lose a life function will only be called after player collides with an obstacle x amount of times each collison will cause the camera to shake
    public void LoseLife(){
        Debug.Log("Active");
        if (timing.songPosition >= 4)
        {
            if (invincible) return;
            gameManager.ClearCombo();
            if (collidedAmout == 0)
            {
                collidedAmout++;
                StartCoroutine(RegainLives());
            }
            else
            {
                collidedAmout++; // Increment the collided amount
            }
            cineMachineNoise.AmplitudeGain += 1;
            cineMachineNoise.FrequencyGain += 1;
            //Debug.Log("Collided Amount: " + collidedAmout); // Log the collided amount
            if (collidedAmout >= maxCollisions)
            { // If the collided amount is greater than or equal to the max collisions
                //Debug.Log("Lives: " + lives);
                Death();
            }
            else
            {
                moveBackwards.forwardSpeed = moveBackwards.minSpeed;
            }
            if (gameManager.lives <= 0)
            {
                if (SceneManager.GetActiveScene().name != "Infinite")
                {
                    gameManager.GameOver();
                }
                else
                {
                    Win win = GameObject.Find("SectionManager").GetComponent<Win>();
                    win.Winning();
                }
            }
        }
    }
    #endregion

    public void Death()
    {
        gameManager.ClearCombo();
        collidedAmout = 0;
        cineMachineNoise.AmplitudeGain = 0;
        cineMachineNoise.FrequencyGain = 0;
        rewind.StartRewind();
        gameManager.lives--;
        livesText.ChangeText();
        if (gameManager.lives <= 0)
        {
            gameManager.GameOver();
        }
    }

    IEnumerator RegainLives()
    {
        yield return new WaitForSeconds(regenTime);
        while (collidedAmout > 0)
        { 
            collidedAmout--;
            // Put sound effect here
            if (cineMachineNoise.AmplitudeGain > 0)
            {
                cineMachineNoise.AmplitudeGain -= 1;
                cineMachineNoise.FrequencyGain -= 1;
            }
            yield return new WaitForSeconds(regenTime);
            //Debug.Log("Regained");
        }
    }

    public void ShakeCamera(int shakeIntensity){
        // shake the camera by using the CinemachineShake script
    } // end of ShakeCamera function
}
