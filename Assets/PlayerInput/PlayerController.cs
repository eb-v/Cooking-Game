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

    public void OnRespawn(InputAction.CallbackContext context)
    {
        if (context.started) // pressed down
        {
            GenericEvent<OnRespawnInput>.GetEvent("RespawnManager").Invoke(gameObject);
        }
    }

    public void OnBoost(InputAction.CallbackContext context) {
        if (context.started) // pressed down
        {
            GenericEvent<OnBoostInput>.GetEvent(gameObject.name).Invoke();
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
            if (env_Interaction.currentlyLookingAt == null) return;
            RagdollController ragdollController = GetComponent<RagdollController>();

            if (!ragdollController.RagdollDict["UpperLeftArm"].isConnected && !ragdollController.RagdollDict["UpperRightArm"].isConnected)
            {
                Debug.Log("Both arms are missing, cannot interact.");
                return;
            }
            if (env_Interaction.currentlyLookingAt.tag != "Player")
            {
                if (env_Interaction.currentlyLookingAt.tag == "Customer")
                {
                    GenericEvent<OnCustomerInteract>.GetEvent(env_Interaction.currentlyLookingAt.GetInstanceID().ToString()).Invoke(gameObject);
                }

                GenericEvent<Interact>.GetEvent(env_Interaction.currentlyLookingAt?.name).Invoke();
                GenericEvent<InteractEvent>.GetEvent(env_Interaction.currentlyLookingAt?.name).Invoke(gameObject);
            }
            else
            {
                // player is looking at other player
                // attempt to reconnect joint
                GenericEvent<InteractEvent>.GetEvent(env_Interaction.currentlyLookingAt.transform.root.name).Invoke(gameObject);
            }
        } else if (context.canceled) {
            IsInteractPressed = false;
            GenericEvent<StopInteract>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnAlternateInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (env_Interaction.currentlyLookingAt != null)
            {
                GenericEvent<AlternateInteractInput>.GetEvent(env_Interaction.currentlyLookingAt.name).Invoke(gameObject);
            }
        }
    }

    public void OnDpadInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (env_Interaction.currentlyLookingAt == null) return;
            Vector2 dpadInput = context.ReadValue<Vector2>();
            if (dpadInput == Vector2.zero) return;
            GenericEvent<DPadInteractEvent>.GetEvent(env_Interaction.currentlyLookingAt.name).Invoke(dpadInput);
        }
    }

    public void OnRemoveObjectFromKitchenProp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (env_Interaction.currentlyLookingAt != null)
            {
                GenericEvent<RemovePlacedObject>.GetEvent(env_Interaction.currentlyLookingAt.name).Invoke(gameObject);
            }
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

    public void OnExplode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GenericEvent<OnExplodeInput>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnNextOption(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GenericEvent<OnNextOptionInput>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnPreviousOption(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GenericEvent<OnPreviousOptionInput>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GenericEvent<OnSelectInput>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GenericEvent<OnNavigateInput>.GetEvent(gameObject.name).Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnReady(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GenericEvent<PlayerReadyInputEvent>.GetEvent("PlayerReady").Invoke(gameObject);
        }
    }
}

