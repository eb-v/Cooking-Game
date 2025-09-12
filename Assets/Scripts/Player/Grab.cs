using UnityEngine;

public class Grab : MonoBehaviour
{
    [SerializeField] private GameObject playerRagdoll;
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;

    private void Awake()
    {
        GenericEvent<OnLeftGrabInput>.GetEvent(playerRagdoll.GetInstanceID()).AddListener(OnLeftGrab);
        GenericEvent<OnRightGrabInput>.GetEvent(playerRagdoll.GetInstanceID()).AddListener(OnRightGrab);
        GenericEvent<OnLeftGrabReleased>.GetEvent(playerRagdoll.GetInstanceID()).AddListener(OnLeftGrabReleased);
        GenericEvent<OnRightGrabReleased>.GetEvent(playerRagdoll.GetInstanceID()).AddListener(OnRightGrabReleased);
    }

    private void OnLeftGrab()
    {
        Vector3 currentRotation = leftArm.localEulerAngles;
        currentRotation.x = 270f;
        leftArm.rotation = Quaternion.Euler(currentRotation);
        Debug.Log("Left Grab");
    }

    private void OnRightGrab()
    {
        Vector3 currentRotation = rightArm.localEulerAngles;
        currentRotation.x = 270f;
        rightArm.rotation = Quaternion.Euler(currentRotation);
        Debug.Log("Right Grab");
    }

    private void OnLeftGrabReleased()
    {
        Vector3 currentRotation = leftArm.localEulerAngles;
        currentRotation.x = 0f;
        leftArm.rotation = Quaternion.Euler(currentRotation);
        Debug.Log("Left Grab Released");
    }

    private void OnRightGrabReleased()
    {
        Vector3 currentRotation = rightArm.localEulerAngles;
        currentRotation.x = 0f;
        rightArm.rotation = Quaternion.Euler(currentRotation);
        Debug.Log("Right Grab Released");
    }
}
