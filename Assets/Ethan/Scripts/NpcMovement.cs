using UnityEngine;
using UnityEngine.AI;

public class NpcMovement : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent; //Navmesh Agent
    NpcManager npcManager; //Npc Manager
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        npcManager = GameObject.Find("NpcManager").GetComponent<NpcManager>();
        agent = GetComponent<NavMeshAgent>(); // get agent
        agent.speed = Random.Range(3, 6);
    }

    void OnEnable()
    {
        if (target != null)
        {
            agent.SetDestination(target.position); // set agent destination to target position
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


// Destroys the npc when it collides with the target
void OnTriggerEnter(Collider other)
{
    
    if (other.CompareTag("Target") && target != null && other.transform == target)
    {
        gameObject.SetActive(false); // destroy npc
    }
}
}