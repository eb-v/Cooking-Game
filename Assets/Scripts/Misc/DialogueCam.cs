using Unity.Cinemachine;
using UnityEngine;

public class DialogueCam : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;

    private const int ActivePriority = 10;
    private const int InactivePriority = 0;

    public void SetCameraActive()
    {
        cam.Priority = ActivePriority;
    }

    public void SetCameraInactive()
    {
        cam.Priority = InactivePriority;
    }
}
