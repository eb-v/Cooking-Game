using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();

            GenericEvent<OnMoveInput>.GetEvent(gameObject.GetInstanceID()).Invoke(moveInput);

            if (moveInput == Vector2.zero)
            {
                GenericEvent<OnWalkStatusChange>.GetEvent(gameObject.GetInstanceID()).Invoke(false);
            }
            else
            {
                GenericEvent<OnWalkStatusChange>.GetEvent(gameObject.GetInstanceID()).Invoke(true);
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) // pressed down
        {
            GenericEvent<OnJumpInput>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
    }

    public void OnLeftGrab(InputAction.CallbackContext context)
    {
        if (context.started) // button pressed
        {
            GenericEvent<OnLeftGrabInput>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnLeftGrabReleased>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
    }

    public void OnRightGrab(InputAction.CallbackContext context)
    {
        if (context.started) // pressed down
        {
            GenericEvent<OnRightGrabInput>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnRightGrabReleased>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
    }
}

