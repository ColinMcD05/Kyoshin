using UnityEngine;

public class Timing : MonoBehaviour
{
    GameObject player;
    Rewind rewind;

    float timePassed;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        
    }

    void IncreaseTimePassed()
    {
        timePassed += Time.deltaTime;
    }
}
