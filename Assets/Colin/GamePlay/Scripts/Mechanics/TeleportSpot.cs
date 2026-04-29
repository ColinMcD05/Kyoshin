using UnityEngine;

public class TeleportSpot : MonoBehaviour
{
    public Transform teleportSpot;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody body = other.GetComponent<Rigidbody>();
            body.position = new Vector3(body.position.x, teleportSpot.position.y, body.position.z);
        }
    }
}
