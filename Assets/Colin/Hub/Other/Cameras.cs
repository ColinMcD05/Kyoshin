using System.Collections;
using UnityEngine;

public class Cameras : MonoBehaviour
{
    // Variables
    #region
    // References
    Camera thisCamera;
    Camera mainCamera;
    Transform orientation;
    GameObject player;
    PlayerHubMovement playerMovement;

    // Rotation
    public int rotationChange;
    Quaternion originalRotation;
    Quaternion newRotation;
    #endregion

   void Start()
    {
        // Set Camera references
        thisCamera = gameObject.GetComponentInParent<Camera>();
        mainCamera = Camera.main;
        player = GameObject.Find("Player_Hub");
        orientation = player.transform.GetChild(0);
        Debug.Log(orientation.gameObject.name);
        playerMovement = player.GetComponent<PlayerHubMovement>();
        originalRotation = orientation.transform.rotation;
        newRotation = originalRotation * Quaternion.Euler(0, rotationChange, 0);
    }

    // When entering area switch camera
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            thisCamera.enabled = true;
            mainCamera.enabled = false;
            StartCoroutine(ChangeRotation(newRotation));
        }
    }

    // When exiting area switch camera
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            thisCamera.enabled = false;
            mainCamera.enabled = true;
            StartCoroutine(ChangeRotation(originalRotation));
        }
    }

    IEnumerator ChangeRotation(Quaternion change)
    {
        while (playerMovement.direction.x != 0 || playerMovement.direction.z != 0)
        {
            yield return null;
        }
        orientation.transform.rotation = change;
    }
}
