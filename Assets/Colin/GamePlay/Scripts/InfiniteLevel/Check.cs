using UnityEngine;

public class Check : MonoBehaviour
{
    // Used to make sure that infinite level spawns correctly
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log(other.GetComponent<Timing>().songPositionInBeats);
        }
    }
}
