using UnityEngine;
using UnityEngine.InputSystem;

public class ArtPrototypeController : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    [SerializeField] InputActionReference look;

    public float xRotation;
    public float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mouseInput = look.action.ReadValue<Vector2>();

        //get mouse or right stick input, seperate to 2 floats for x and y.
        float mouseX = mouseInput.x * sensX * Time.deltaTime;
        float mouseY = mouseInput.y * sensY * Time.deltaTime;
        //use inputs to turn
        xRotation += mouseX;
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(yRotation, xRotation, 0);
        orientation.rotation = Quaternion.Euler(0, xRotation, 0);
    }
}
