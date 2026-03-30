using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public GameObject playerStartPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            other.gameObject.transform.position = playerStartPosition.transform.position;
        }
    }
}
