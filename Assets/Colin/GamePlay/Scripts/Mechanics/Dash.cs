using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    Slider dashSlider;

    AudioSource dashSource;
    public AudioClip dashSound;

    void Start()
    {
        dashSlider = GameObject.Find("GameManager").transform.Find("Canvas").transform.Find("Dash").GetComponent<Slider>();
        dashSource = GameObject.Find("Audio").transform.Find("SoundEffects").GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (dashing)
        {
            dashMeter -= Time.deltaTime / dashBarLength;
            dashSlider.value -= Time.deltaTime / dashBarLength;

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
        if (dashMeter >= 1 && !rewind.rewinding)
        {
            dashing = !dashing;
            // Player should still move forward, but not have any control
            //playerControllerLevel.enabled = !playerControllerLevel.enabled;
            if (dashing)
            {
                // Play dash sound
                dashSource.PlayOneShot(dashSound);
                moveBackwards.forwardSpeed *= dashMult;
                playerMovement.currentLane = 1;
                playerController.enabled = false;
                timing.UnSubscribeActions();
                playerMovement.UnSubscribeActions();
                playerMovement.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
            else
            {
                // Stop dash sound
                dashSource.Stop();
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
