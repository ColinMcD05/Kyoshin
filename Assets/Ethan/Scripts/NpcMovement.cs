using UnityEngine;
using UnityEngine.AI;

public class NpcMovement : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent; //Navmesh Agent
    public NpcManager npcManager; //Npc Manager
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // get agent
        if(target != null){
            agent.SetDestination(target.position); // set agent destination to target position
            npcManager.currentTarget = target; // set current target to target
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


// Destroys the npc when it collides with the target
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Target") && npcManager.currentTarget == target)
    {
        Destroy(gameObject); // destroy npc
    }
}
}