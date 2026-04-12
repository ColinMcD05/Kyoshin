using UnityEngine;

public class Cameras : MonoBehaviour
{
    // Variables
    #region
    // References
    Camera thisCamera;
    Camera mainCamera;
    #endregion

   void Start()
    {
        // Set Camera references
        thisCamera = gameObject.GetComponentInParent<Camera>();
        mainCamera = Camera.main;
    }

    // When entering area switch camera
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            thisCamera.enabled = true;
            mainCamera.enabled = false;
        }
    }

    // When exiting area switch camera
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            thisCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }
}
