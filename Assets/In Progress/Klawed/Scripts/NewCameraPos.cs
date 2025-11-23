using UnityEngine;

public class NewCameraPos : MonoBehaviour
{
    [SerializeField] private Transform lookTarget;
    
    public void CallMoveCamera()
    {
        CameraController camController = FindFirstObjectByType<CameraController>();
        camController.MoveCameraTo(transform, lookTarget);
    }

}
