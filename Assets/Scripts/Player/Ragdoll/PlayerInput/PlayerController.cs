using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            // Debug.Log("Move Input: " + moveInput);

            GenericEvent<OnMoveInput>.GetEvent(gameObject.name).Invoke(moveInput);

            //if (moveInput == Vector2.zero)
            //{
            //    GenericEvent<OnWalkStatusChange>.GetEvent(inputChannel).Invoke(false);
            //}
            //else
            //{
            //    GenericEvent<OnWalkStatusChange>.GetEvent(inputChannel).Invoke(true);
            //}
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.started) // pressed down
        {
            GenericEvent<OnJumpInput>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnLeftGrab(InputAction.CallbackContext context)
    {
        if (context.started) // button pressed
        {
            GenericEvent<OnLeftGrabInput>.GetEvent(gameObject.name).Invoke(true);
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnLeftGrabInput>.GetEvent(gameObject.name).Invoke(false);
        }
    }

    public void OnRightGrab(InputAction.CallbackContext context)
    {
        if (context.started) // pressed down
        {
            GenericEvent<OnRightGrabInput>.GetEvent(gameObject.name).Invoke(true);
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnRightGrabInput>.GetEvent(gameObject.name).Invoke(false);
        }
    }



    public void OnLeanForwards(InputAction.CallbackContext context)
    {
        if (context.started) // pressed down
        {
            GenericEvent<OnLeanForwardInput>.GetEvent(gameObject.name).Invoke();
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnLeanForwardCancel>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnLeanBackwards(InputAction.CallbackContext context)
    {
        if (context.started) // pressed down
        {
            GenericEvent<OnLeanBackwardInput>.GetEvent(gameObject.name).Invoke();
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnLeanBackwardCancel>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.started)
        {

           //Debug.Log("F Pressed");
            GenericEvent<Interact>.GetEvent(gameObject.name).Invoke();

        } else if (context.canceled)
        {
            //Debug.Log("F not");
            GenericEvent<StopInteract>.GetEvent(gameObject.name).Invoke();

        }
    }

    public void OnDetachBodyInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //GenericEvent<OnDetatchJoint>.GetEvent(bodyChannel).Invoke();
        }
    }

    public void OnReattachBodyInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //GenericEvent<OnReattachJoint>.GetEvent(bodyChannel).Invoke();
        }
    }

}

