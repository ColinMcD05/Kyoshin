using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    public float dashMeter;
    public int dashMax;
    public int dashMult = 2;
    public float dashBarLength;
    public bool dashing;
    [SerializeField] PlayerControllerLevel playerControllerLevel;

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
            playerControllerLevel.forwardSpeed /= dashMult;
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
                playerControllerLevel.forwardSpeed *= dashMult;
            }
            else
            {
                playerControllerLevel.forwardSpeed /= dashMult;
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
