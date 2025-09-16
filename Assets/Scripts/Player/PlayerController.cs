using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject leftHandObj;
    [SerializeField] private GameObject rightHandObj;

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
            GenericEvent<OnGrabInput>.GetEvent(leftHandObj.GetInstanceID()).Invoke();
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnGrabReleased>.GetEvent(leftHandObj.GetInstanceID()).Invoke();
        }
    }

    public void OnRightGrab(InputAction.CallbackContext context)
    {
        if (context.started) // pressed down
        {
            GenericEvent<OnGrabInput>.GetEvent(rightHandObj.GetInstanceID()).Invoke();
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnGrabReleased>.GetEvent(rightHandObj.GetInstanceID()).Invoke();
        }
    }

    public void OnLeanBackwards(InputAction.CallbackContext context)
    {
        if (context.performed) // pressed down
        {
            GenericEvent<OnLeanBackwardsHold>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnLeanBackwardsCancel>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
    }

    public void OnLeanForwards(InputAction.CallbackContext context)
    {
        if (context.performed) // pressed down
        {
            GenericEvent<OnLeanForwardsHold>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnLeanForwardsCancel>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
    }

}

