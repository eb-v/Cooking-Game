using UnityEngine;

public class GrabLogic : MonoBehaviour
{
    private bool isGrabbing = false;
    private GameObject grabbedObject;
    private FixedJoint grabJoint;

    private void Awake()
    {
        GenericEvent<OnHandCollisionEnter>.GetEvent(gameObject.GetInstanceID()).AddListener(GrabObject);
        //GenericEvent<OnHandCollisionExit>.GetEvent(gameObject.GetInstanceID()).AddListener(ReleaseObject);
        GenericEvent<OnGrabReleased>.GetEvent(gameObject.GetInstanceID()).AddListener(ReleaseObject);
    }

    private void GrabObject(GameObject obj)
    {
        if (!isGrabbing)
        {
            grabbedObject = obj;
            grabJoint = grabbedObject.AddComponent<FixedJoint>();
            grabJoint.connectedBody = transform.parent.gameObject.GetComponent<Rigidbody>();
            // set breakforce to infinity
            grabJoint.breakForce = Mathf.Infinity;
            isGrabbing = true;
        }
    }

    private void ReleaseObject()
    {
        if (isGrabbing)
        {
            Destroy(grabJoint);
            grabbedObject = null;
            isGrabbing = false;
        }
    }

    public GameObject GetGrabbedObject() {
        return isGrabbing ? grabbedObject : null;
    }
}
