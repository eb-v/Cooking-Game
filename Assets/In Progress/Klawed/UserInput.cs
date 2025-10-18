using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public void OnMoveImage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GenericEvent<MoveImageEvent>.GetEvent("me").Invoke(true);
        }
        else if (context.canceled)
        {
            GenericEvent<MoveImageEvent>.GetEvent("me").Invoke(false);
        }
    }

    
}
