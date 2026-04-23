using UnityEngine;
using UnityEngine.AI;

public class DelaMovementHub : MonoBehaviour
{
    public Transform target;
    public Vector3 playerTarget;
    private NavMeshAgent agent; //Navmesh Agent
    public NpcManager npcManager; //Npc Manager
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // get agent
        if (target != null)
        {
            agent.SetDestination(target.position); // set agent destination to target position

        }
        playerTarget = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        // checks if the agent is moving if its not moving that means it has reached it target 
        // Need a way to reset the has reached value once the player starts moving again
        if(agent.pathPending == false)
        {
            Debug.Log("Dela reached Revi");
            agent.isStopped = false;
            ResetPath();
            
        }

    }
    public void ResetPath()
    {
        agent.SetDestination(target.position);
    }


}