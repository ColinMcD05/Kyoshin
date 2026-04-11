using UnityEngine;

public class Cameras : MonoBehaviour
{
    Camera thisCamera;
    Camera mainCamera;

    void Start()
    {
        thisCamera = gameObject.GetComponentInParent<Camera>();
        mainCamera = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            thisCamera.enabled = true;
            mainCamera.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            thisCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }
}
