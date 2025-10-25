using UnityEngine;

public class PlayerReadyScript : MonoBehaviour
{
    [SerializeField] private SpringAPI readyImageSpring;
    [SerializeField] private SpringAPI readyTxtSpring;
    [SerializeField] private string _assignedChannel;
    [SerializeField] private bool _ready;

    private void Start()
    {
        GenericEvent<PlayerReadyEvent>.GetEvent(_assignedChannel).AddListener(ChangeReadyStatus);
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

    private void OnPlayerReady()
    {
        readyImageSpring.SetGoalValue(1f);
        readyTxtSpring.SetGoalValue(1f);
    }

    private void OnPlayerUnReady()
    {
        readyImageSpring.SetGoalValue(0f);
        readyTxtSpring.SetGoalValue(0f);
        readyImageSpring.ResetSpring();
        readyTxtSpring.ResetSpring();
    }

}
