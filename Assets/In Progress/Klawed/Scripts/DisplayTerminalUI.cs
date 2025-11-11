using UnityEngine;

public class DisplayTerminalUI : MonoBehaviour
{
    [SerializeField] private SpringAPI springAPI;

    private void Start()
    {
        GenericEvent<PlayerLookingAtObject>.GetEvent(gameObject.name).AddListener(OnPlayerLookingAtObject);
        GenericEvent<PlayerStoppedLookingAtObject>.GetEvent(gameObject.name).AddListener(TurnOffTerminal);
    }

    private void OnPlayerLookingAtObject()
    {
        DisplayTerminal();
    }

    private void DisplayTerminal()
    {
        springAPI.SetGoalValue(1f);
    }

    private void TurnOffTerminal()
    {
        springAPI.SetGoalValue(0f);
    }
}
