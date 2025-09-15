using UnityEngine;

public class GrabLogic : MonoBehaviour
{
    [SerializeField] private Rigidbody _armRb;
    private GameObject grabbedObject;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Grabbable"))
        {
            grabbedObject = collider.gameObject;
            FixedJoint fj = grabbedObject.AddComponent<FixedJoint>();
            fj.connectedBody = _armRb;
            fj.breakForce = 9000;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == grabbedObject)
        {
            Destroy(grabbedObject.GetComponent<FixedJoint>());
            grabbedObject = null;
        }
    }

    private void OnDisable()
    {
        if (grabbedObject != null)
        {
            Destroy(grabbedObject.GetComponent<FixedJoint>());
            grabbedObject = null;
        }
    }
}
