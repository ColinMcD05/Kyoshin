using UnityEngine;

public class Check : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log(other.GetComponent<Timing>().songPositionInBeats);
        }
    }
}
