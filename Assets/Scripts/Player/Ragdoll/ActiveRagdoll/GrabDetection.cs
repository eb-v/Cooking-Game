using UnityEngine;

public class GrabDetection : MonoBehaviour
{
    [HideInInspector] public bool isGrabbing = false;
    public GameObject grabbedObj;

    private void OnTriggerEnter(Collider other)
    {
            GenericEvent<OnHandCollisionEnter>.GetEvent(transform.root.name).Invoke(gameObject ,other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
            //GenericEvent<OnHandCollisionExit>.GetEvent(inputChannel).Invoke();
    }
}
