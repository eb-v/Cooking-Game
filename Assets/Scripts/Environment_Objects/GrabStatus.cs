using UnityEngine;

public class GrabStatus : MonoBehaviour
{
    private bool isGrabbed = false;

    public void SetGrabbedStatus(bool status)
    {
        isGrabbed = status;
    }

    public bool IsGrabbed()
    {
        return isGrabbed;
    }
}
