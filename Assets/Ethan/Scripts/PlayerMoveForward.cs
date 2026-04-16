using UnityEngine;

public class PlayerMoveForward : MonoBehaviour
{
    Rigidbody playerRigidbody;

    // Forward Speed Variables
    public float forwardSpeed = 8.0f;
    [HideInInspector] public float maxSpeed;
    [HideInInspector] public float minSpeed;

    Vector3 playerVelocity;

    void Start()
    {
        maxSpeed = forwardSpeed * 4;
        minSpeed = forwardSpeed;
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        playerVelocity = transform.forward * forwardSpeed;
        playerVelocity.y = playerRigidbody.linearVelocity.y;
        playerRigidbody.linearVelocity = playerVelocity;
    }
}
