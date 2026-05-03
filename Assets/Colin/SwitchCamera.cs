using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchCamera : MonoBehaviour
{
    public Camera[] cameras;
    public InputActionReference switchCameras;

    void Start()
    {
        cameras[0] = Camera.main;
        switchCameras.action.performed += SwitchingCamera;
    }

    private void OnDestroy()
    {
        switchCameras.action.performed -= SwitchingCamera;
    }

    void SwitchingCamera(InputAction.CallbackContext context)
    {
        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
        }
        string keyPressed = context.control.name;

        switch (keyPressed)
        {
            default:
            case "1":
                cameras[0].enabled = true;
                break;
            case "2":
                cameras[1].enabled = true;
                break;
            case "3":
                cameras[2].enabled = true;
                break;
            case "4":
                cameras[3].enabled = true;
                break;
            case "5":
                cameras[4].enabled = true;
                break;
        }
    }
}
