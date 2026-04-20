using UnityEngine;

public class Ramp : MonoBehaviour
{
    public float addedForce;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 forceAdded = new Vector3(0, addedForce, 0);
            other.GetComponent<Rigidbody>().AddForce(forceAdded, ForceMode.Impulse);
        }
    }
}
