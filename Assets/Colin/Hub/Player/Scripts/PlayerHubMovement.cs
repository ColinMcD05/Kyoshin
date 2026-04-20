using UnityEngine;

public class PlayerHubMovement : MonoBehaviour
{

    public int moveSpeed;
    public int rotationSpeed;
    public Vector3 direction;
    [SerializeField] Transform orientation;

    void Start()
    {
        
    }


    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);
    }

    public void SetDirection(Vector2 movement)
    {
        Vector3 xInput = movement.x * orientation.right;
        Vector3 yInput = movement.y * orientation.forward;
        direction = xInput + yInput;
    }
}
