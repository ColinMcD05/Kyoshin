using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    public float dashMeter;
    public int dashMax;
    public int dashMult = 2;
    public float dashBarLength;
    public bool dashing;
    [SerializeField] MoveBackwards moveBackwards;
    [SerializeField] PlayerLevelMovement playerMovement;
    [SerializeField] PlayerControllerLevel playerController;
    [SerializeField] Timing timing;
    [SerializeField] Rewind rewind;

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
            moveBackwards.forwardSpeed /= dashMult;
            rewind.Invoke("BecomeVulnerable", rewind.invincibility);
            timing.SubscribeActions();
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
                moveBackwards.forwardSpeed *= dashMult;
                playerMovement.currentLane = 1;
                playerController.enabled = false;
                timing.UnSubscribeActions();
                playerMovement.UnSubscribeActions();
            }
            else
            {
                moveBackwards.forwardSpeed /= dashMult;
                rewind.Invoke("BecomeVulnerable", rewind.invincibility);
                timing.SubscribeActions();
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
