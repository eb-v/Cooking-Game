using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    public void OnMove(InputValue value)
    {
        GenericEvent<OnMoveInput>.GetEvent(gameObject.GetInstanceID()).Invoke(value.Get<Vector2>());
    }

    //public void OnLook(InputValue value)
    //{
    //    GenericEvent<OnLookInput>.GetEvent(gameObject.GetInstanceID()).Invoke(value.Get<Vector2>());
    //}

}
