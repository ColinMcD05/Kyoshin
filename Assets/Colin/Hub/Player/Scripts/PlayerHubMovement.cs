using UnityEngine;

public class PlayerHubMovement : MonoBehaviour
{

    public int moveSpeed;
    public int rotationSpeed;
    public Vector3 direction;

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
        direction = new Vector3(movement.x, 0, movement.y);
    }
}
