using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    private Env_Interaction env_Interaction;


    private void Awake()
    {
        env_Interaction = GetComponent<Env_Interaction>();
    }

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
    public bool IsInteractPressed { get; private set; } = false;

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.started) {
            IsInteractPressed = true;
            GenericEvent<Interact>.GetEvent(gameObject.name).Invoke();
        } else if (context.canceled) {
            IsInteractPressed = false;
            GenericEvent<StopInteract>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnPerformSkillCheck(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (env_Interaction.currentlyLookingAt != null)
            {
                GenericEvent<SkillCheckInput>.GetEvent(env_Interaction.currentlyLookingAt.name).Invoke(gameObject);
            }
        }
    }

    public void OnRemoveObjectFromKitchenProp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GenericEvent<RemovePlacedObject>.GetEvent(gameObject.name).Invoke();
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

    public void OnPlaceIngredient(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GenericEvent<OnPlaceIngredientInput>.GetEvent(gameObject.name).Invoke();
        }
    }

}

