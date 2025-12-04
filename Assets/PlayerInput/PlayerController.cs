using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    //private Env_Interaction env_Interaction;
    //private GameObject lookedAtObj => env_Interaction.currentlyLookingAt;


    private void Awake()
    {
        
    }

    

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            GenericEvent<OnMoveInput>.GetEvent(gameObject.name).Invoke(moveInput);
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

    public void OnPerformStationAction(InputAction.CallbackContext context)
    {
        if (context.performed) // pressed down
        {
            GenericEvent<OnPerformStationAction>.GetEvent(gameObject.name).Invoke();
        }
        else if (context.canceled) // button released
        {
            GenericEvent<OnPerformStationActionCancel>.GetEvent(gameObject.name).Invoke();
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

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GenericEvent<OnInteractInput>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnAlternateInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GenericEvent<OnAlternateInteractInput>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GenericEvent<OnGrabInputEvent>.GetEvent(gameObject.name).Invoke();
        }
    }

    public void OnThrowInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GenericEvent<OnThrowStatusChanged>.GetEvent(gameObject.name).Invoke(true);
        }
        else if (context.canceled)
        {
            GenericEvent<OnThrowStatusChanged>.GetEvent(gameObject.name).Invoke(false);
        }

    }

    public void OnDpadInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 dpadInput = context.ReadValue<Vector2>();
            if (dpadInput == Vector2.zero) return;
            GenericEvent<DPadInteractEvent>.GetEvent(gameObject.name).Invoke(dpadInput);
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

    public void OnUseEquipment(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GenericEvent<OnEquipmentUseInput>.GetEvent(gameObject.name).Invoke(true);
        }
        else if (context.canceled)
        {
            GenericEvent<OnEquipmentUseInput>.GetEvent(gameObject.name).Invoke(false);
        }
    }

    public void OnPauseGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GenericEvent<OnPauseGameInput>.GetEvent("PauseManager").Invoke();
        }
    }
}

