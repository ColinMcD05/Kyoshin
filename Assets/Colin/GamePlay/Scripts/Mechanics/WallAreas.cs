using Unity.VisualScripting;
using UnityEngine;

public class WallAreas : MonoBehaviour
{
    public PlayerLevelMovement.AreaType areaType;

    public Transform leftWallPosition;
    public Transform rightWallPosition;

    public void Start()
    {
        if (leftWallPosition == null && rightWallPosition == null)
        {
            GameObject sectionManager = GameObject.Find("SectionManager");
            if (sectionManager == null) return;
            switch (areaType)
            {
                default:
                case PlayerLevelMovement.AreaType.wallRunning:
                    leftWallPosition = sectionManager.transform.Find("LeftWall");
                    rightWallPosition = sectionManager.transform.Find("RightWall");
                    break;
                case PlayerLevelMovement.AreaType.closeWallRunning:
                    leftWallPosition = sectionManager.transform.Find("CloseLeftWall");
                    rightWallPosition = sectionManager.transform.Find("CloseRightWall");
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // Once player enters wall running area, it sets the position of where the walls are, and what type.
            PlayerLevelMovement playerMovement = other.GetComponent<PlayerLevelMovement>();
            if (leftWallPosition != null)
            {
                playerMovement.leftWallPosition = leftWallPosition.position;
            }
            if (rightWallPosition != null)
            {
                playerMovement.rightWallPosition = rightWallPosition.position;
            }
            playerMovement.areaType = areaType;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Once player exits area, sets player back to normal position
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerLevelMovement playerMovement = other.GetComponent<PlayerLevelMovement>();
            playerMovement.leftWallPosition = null;
            playerMovement.rightWallPosition = null;
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
