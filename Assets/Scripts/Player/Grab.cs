using UnityEngine;

public class Grab : MonoBehaviour
{
    private ConfigurableJoint _armJoint;

    private void Awake()
    {
        GenericEvent<OnLeftGrabInput>.GetEvent(gameObject.GetInstanceID()).AddListener(OnLeftGrab);
        GenericEvent<OnRightGrabInput>.GetEvent(gameObject.GetInstanceID()).AddListener(OnRightGrab);
        GenericEvent<OnLeftGrabReleased>.GetEvent(gameObject.GetInstanceID()).AddListener(OnLeftGrabReleased);
        GenericEvent<OnRightGrabReleased>.GetEvent(gameObject.GetInstanceID()).AddListener(OnRightGrabReleased);
        _armJoint = GetComponent<ConfigurableJoint>();
    }

    private void OnLeftGrab()
    {
    }

    private void OnRightGrab()
    {
       
    }

    private void OnLeftGrabReleased()
    {
        
    }

    private void OnRightGrabReleased()
    {
        
    }
}
