using UnityEngine;

public class Ramp : MonoBehaviour
{
    public float addedForce;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<Rigidbody>().AddForce(Vector3.up*addedForce, ForceMode.Impulse);
        }
    }
}
