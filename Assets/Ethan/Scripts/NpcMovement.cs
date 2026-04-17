using UnityEngine;
using UnityEngine.AI;

public class NpcMovement : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent; //Navmesh Agent
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // get agent
        if(target != null){
            agent.SetDestination(target.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check if agent has reached end
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                //if agent does not have a path or is not moving destroy npc 
                Destroy(gameObject);
            }
        }
    }
}
