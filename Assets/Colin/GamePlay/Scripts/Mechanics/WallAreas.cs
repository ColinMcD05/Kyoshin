using Unity.VisualScripting;
using UnityEngine;

public class WallAreas : MonoBehaviour
{
    public PlayerLevelMovement.AreaType areaType;

    public Transform leftWallPosition;
    public Transform rightWallPosition;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // Once player enters wall running area, it sets the position of where the walls are, and what type.
            PlayerLevelMovement playerMovement = other.GetComponent<PlayerLevelMovement>();
            playerMovement.leftWallPosition = leftWallPosition.position;
            playerMovement.rightWallPosition = rightWallPosition.position;
            playerMovement.areaType = areaType;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Once player exits area, sets player back to normal position
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerLevelMovement playerMovement = other.GetComponent<PlayerLevelMovement>();
            playerMovement.leftWallPosition = Vector2.zero;
            playerMovement.rightWallPosition = Vector2.zero;
            playerMovement.areaType = PlayerLevelMovement.AreaType.normal;
            if (!playerMovement.IsGrounded())
            {
                switch (areaType)
                {
                    default:
                    case PlayerLevelMovement.AreaType.closeWallRunning:
                            playerMovement.currentLane = 1;
                        break;
                        
                    case PlayerLevelMovement.AreaType.wallRunning:
                        {
                            switch (playerMovement.wallType)
                            {
                                default:
                                case PlayerLevelMovement.WallType.none:
                                    break;
                                case PlayerLevelMovement.WallType.rightWall:
                                    playerMovement.currentLane = 2;
                                    break;
                                case PlayerLevelMovement.WallType.leftWall:
                                    playerMovement.currentLane = 0;
                                    break;
                            }
                            break;
                        }
                }
            }
        }
    }
}
