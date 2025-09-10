using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    public void OnMove(InputValue value)
    {
        GenericEvent<OnMoveInput>.GetEvent(gameObject.GetInstanceID()).Invoke(value.Get<Vector2>());
        //if (value.Get<Vector2>() == Vector2.zero)
        //{
        //   // GenericEvent<Idle>.GetEvent(gameObject.GetInstanceID()).Invoke();

        //}
        //else
        //{
        //    GenericEvent<Move>.GetEvent(gameObject.GetInstanceID()).Invoke();
        //}
    }

    public void OnJump()
    {
        GenericEvent<OnJumpInput>.GetEvent(gameObject.GetInstanceID()).Invoke();
    }

}
