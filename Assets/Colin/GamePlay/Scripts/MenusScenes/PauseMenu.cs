using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private float lastToggleTime;
    private float toggleCooldown = 0.2f;
    [SerializeField] InputActionReference pause;

    private void Awake()
    {
        pause.action.performed += PausePerformed;
    }

    private void OnDestroy()
    {
        pause.action.performed -= PausePerformed;
    }

    void PausePerformed(InputAction.CallbackContext context)
    {
        if (Time.realtimeSinceStartup - lastToggleTime < toggleCooldown) return;

        lastToggleTime = Time.realtimeSinceStartup;

        if (Time.timeScale != 0.0001f)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
    public void Pause()
    {
        Time.timeScale = 0.0001f;
        AudioListener.pause = true;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
    }
}
