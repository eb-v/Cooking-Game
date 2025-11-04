using UnityEngine;

public class PlayerReadyScript : MonoBehaviour
{
    [SerializeField] private Transform readyUITransform;
    [SerializeField] private string _assignedChannel;
    [SerializeField] private bool _ready;
    [SerializeField] private float _readyScale = 1.34f;
    [SerializeField] private CheckIfAllPlayersReady _checkIfAllPlayersReady;

    private void Awake()
    {
        GenericEvent<PlayerReadyInputEvent>.GetEvent(_assignedChannel).AddListener(ChangeReadyStatus);
    }

    private void ChangeReadyStatus()
    {
        _ready = !_ready;
        if (_ready)
        {
            OnPlayerReady();
        }
        else
        {
            OnPlayerUnReady();
        }
    }

    public void OnPlayerReady()
    {
        readyUITransform.localScale = _readyScale * Vector3.one;
        _checkIfAllPlayersReady.IncrementReady();
    }

    private void OnPlayerUnReady()
    {
        readyUITransform.localScale = Vector3.zero;
        _checkIfAllPlayersReady.DecrementReady();
    }

}
