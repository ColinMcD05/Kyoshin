using UnityEngine;

public class Cameras : MonoBehaviour
{
    public Camera thisCamera;
    public Camera mainCamera;

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
