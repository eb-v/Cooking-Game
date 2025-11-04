using UnityEngine;

public class CameraMotionScript : MonoBehaviour
{
    [SerializeField] private Transform targetEndTransform;
    [SerializeField] private string assignedChannel;
    [SerializeField] private SpringAPI springAPI;

    private void Awake()
    {
        GenericEvent<OnAllPlayersReadyEvent>.GetEvent(assignedChannel).AddListener(OnAllPlayersReady);
    }

    private void OnAllPlayersReady()
    {
        springAPI.SetGoalValue(1f);
    }
}
