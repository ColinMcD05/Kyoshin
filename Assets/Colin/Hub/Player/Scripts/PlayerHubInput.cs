using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // Input Actions needed for Hub movement and actions
    public InputActionReference move;
    public InputActionReference look;

    // Other script references
    public PlayerHubMovement player;

    void Awake()
    {
        move.action.performed += MovePerformed;
        move.action.canceled += MoveCancel;
    }

    private void OnDestroy()
    {
        move.action.performed -= MovePerformed;
        move.action.canceled -= MoveCancel;
    }

    void MovePerformed(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();
        Debug.Log(movement);
        player.SetDirection(movement);
    }

    void MoveCancel(InputAction.CallbackContext context)
    {
        player.SetDirection(Vector2.zero);
    }

    void CameraPerform(InputAction.CallbackContext context)
    {

    }

    void CameraCancel(InputAction.CallbackContext context)
    {

    }
}
