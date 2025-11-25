using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineCamera vcam;

    public void MoveCameraTo(Transform newCamPos, Transform newLookTarget)
    {
        vcam.Follow = newCamPos;
        vcam.LookAt = newLookTarget;
        Debug.Log("Camera moved to new position and look target.");
    }

}
