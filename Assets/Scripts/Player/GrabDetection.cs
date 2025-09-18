using UnityEngine;

public class GrabDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            GenericEvent<OnHandCollisionEnter>.GetEvent(gameObject.GetInstanceID()).Invoke(other.gameObject);
            Debug.Log("Collided with grabbable object: " + other.gameObject.name);  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            //GenericEvent<OnHandCollisionExit>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
    }
}
