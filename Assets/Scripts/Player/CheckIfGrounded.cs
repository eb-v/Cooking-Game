using UnityEngine;

public class CheckIfGrounded : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GenericEvent<GroundedStatusChanged>.GetEvent(transform.root.gameObject.GetInstanceID()).Invoke(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        GenericEvent<GroundedStatusChanged>.GetEvent(transform.root.gameObject.GetInstanceID()).Invoke(false);
    }
}
