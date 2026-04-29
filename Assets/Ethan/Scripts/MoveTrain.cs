using UnityEngine;

public class MoveTrain : MonoBehaviour
{
    public float speed = 50f;

    [Tooltip("End of the run in the +Z direction (should be a fixed world / track object, not parented under this train).")]
    public Transform PosZ;

    [Tooltip("End of the run in the -Z direction (same as PosZ: stationary in the scene).")]
    public Transform NegZ;

    [Tooltip("Distance to a marker at which the train turns around.")]
    public float arriveThreshold = 0.25f; //Distance to a marker at which the train turns around.

    Transform target; //Target position to move towards.

    void Start()
    {
        if (PosZ == null || NegZ == null)
        {
            Debug.Log("MoveTrain needs both PosZ and NegZ assigned.");
            return;
        }

        // Start by running toward the +Z marker (swap in the Inspector if your spawn is on the other end).
        target = PosZ;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (target == null)
        {
            return;
        }
        // Move the train towards the target position, Vector3.MoveTowards is a method that moves the train towards the target position at the speed specified.
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        // transform.position is current position of the train, target.position is the target position, sqrMagnitude is the square distance between the two, arriveThreshold is the distance to a marker at which the train turns around. If the square distance is less than or equal to the square of the arriveThreshold, the train will turn around.
        if ((transform.position - target.position).sqrMagnitude <= arriveThreshold * arriveThreshold) //If the train is close enough to the target position, turn around.
        {
            transform.position = NegZ.position;
        }
    }
}
