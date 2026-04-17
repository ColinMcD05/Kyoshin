using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    public float dashMeter;
    public int dashMax;
    public int dashMult = 2;
    public float dashBarLength;
    public bool dashing;
    [SerializeField] PlayerMoveForward playerForward;
    [SerializeField] PlayerLevelMovement playerMovement;
    [SerializeField] Timing timing;

    private void Update()
    {
        if (dashing)
        {
            dashMeter -= Time.deltaTime / dashBarLength;
        }
        if (dashMeter <= 0 && dashing)
        {
            dashMeter = 0;
            dashing = false;
            playerForward.forwardSpeed /= dashMult;
            playerMovement.leftRight.action.performed += timing.CheckTime;
            playerMovement.jump.action.performed += timing.CheckTime;
            playerMovement.slide.action.performed += timing.CheckTime;
            playerMovement.leftRight.action.performed += playerMovement.LeftRight;
            playerMovement.jump.action.performed += playerMovement.Jump;
            playerMovement.slide.action.performed += playerMovement.Slide;
        }
    }

    public void OnDash(InputValue input)
    {
        if (dashMeter >= 1)
        {
            dashing = !dashing;
            // Player should still move forward, but not have any control
            //playerControllerLevel.enabled = !playerControllerLevel.enabled;
            if (dashing)
            {
                playerForward.forwardSpeed *= dashMult;
                playerMovement.currentLane = 1;
                playerMovement.leftRight.action.performed -= timing.CheckTime;
                playerMovement.jump.action.performed -= timing.CheckTime;
                playerMovement.slide.action.performed -= timing.CheckTime;
                playerMovement.leftRight.action.performed -= playerMovement.LeftRight;
                playerMovement.jump.action.performed -= playerMovement.Jump;
                playerMovement.slide.action.performed -= playerMovement.Slide;
            }
            else
            {
                playerForward.forwardSpeed /= dashMult;
                playerMovement.leftRight.action.performed += timing.CheckTime;
                playerMovement.jump.action.performed += timing.CheckTime;
                playerMovement.slide.action.performed += timing.CheckTime;
                playerMovement.leftRight.action.performed += playerMovement.LeftRight;
                playerMovement.jump.action.performed += playerMovement.Jump;
                playerMovement.slide.action.performed += playerMovement.Slide;
            }
        }
    }

    public void AddDash(float added)
    {
        if (dashMeter >= dashMax)
        {
            return;
        }
        else if (dashMeter+added > dashMax)
        {
            dashMeter = dashMax;
        }
        else if (dashMeter < dashMax)
        {
            dashMeter += added;
        }
    }
}
