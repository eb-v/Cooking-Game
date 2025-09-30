using UnityEngine;

public class GrabDetection : MonoBehaviour
{
    [HideInInspector] public bool isGrabbing = false;
    public GameObject grabbedObj;

    private void OnTriggerEnter(Collider other)
    {
        string layerName = LayerMask.LayerToName(other.gameObject.layer);
        if (layerName == "Player")
        {
            GameObject rootPlayerObj = other.transform.root.gameObject;
            if (rootPlayerObj == gameObject.transform.root.gameObject) 
            {
                return;
            }
        }

        GenericEvent<OnHandCollisionEnter>.GetEvent(transform.root.name).Invoke(gameObject ,other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
            //GenericEvent<OnHandCollisionExit>.GetEvent(inputChannel).Invoke();
    }
}
