using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public void OnMoveImage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GenericEvent<UpdateGoalValueEvent>.GetEvent("me").Invoke(1f);
            GenericEvent<OnButtonPressedEvent>.GetEvent("me").Invoke();
        }
        else if (context.canceled)
        {
            GenericEvent<UpdateGoalValueEvent>.GetEvent("me").Invoke(0.5f);
        }
    }

    
}
