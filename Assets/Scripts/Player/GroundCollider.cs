using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    private bool isAirborne = false;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isAirborne = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && isAirborne)
        {
            isAirborne = false;
            GenericEvent<HasLanded>.GetEvent(transform.root.GetInstanceID()).Invoke();
        }
    }
}
