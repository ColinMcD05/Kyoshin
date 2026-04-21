using UnityEngine;


public class Obstacle : MonoBehaviour
{
    public ObstacleType type;
    public enum ObstacleType
    {
        Kill,
        Hurt
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player") /*&& !other.GetComponent<Dash>().dashing*/)
        {
            if (!other.GetComponent<Dash>().dashing)
            {
                Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
                PlayerLevelMovement playerMovement = other.GetComponent<PlayerLevelMovement>();
                PlayerControllerLevel playerController = other.GetComponent<PlayerControllerLevel>();
                switch (type)
                {
                    case ObstacleType.Kill:
                        if (playerRigidbody.linearVelocity.y >= -0.001)
                        {
                            if (playerRigidbody.linearVelocity.x > 0.01)
                            {
                                playerController.LoseLife();
                                playerMovement.currentLane--;
                                return;
                            }
                            else if (playerRigidbody.linearVelocity.x < -0.01)
                            {
                                playerController.LoseLife();
                                playerMovement.currentLane++;
                                return;
                            }
                        }
                        playerController.Death();
                        break;
                    case ObstacleType.Hurt:
                        playerController.LoseLife();
                        break;
                }
            }
        }
    } // end of OnTriggerEnter function
}
