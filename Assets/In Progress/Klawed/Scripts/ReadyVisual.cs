using UnityEngine;

public class ReadyVisual : MonoBehaviour
{
    [SerializeField] private GameObject readyVisual;
    [SerializeField] private LobbyUIEventHandler lobbyUIEventHandler;

    private void OnEnable()
    {
        GenericEvent<PlayerReadyInputEvent>.GetEvent("PlayerReady").AddListener(OnPlayerReadyInput);
    }

    private void OnDisable()
    {
        GenericEvent<PlayerReadyInputEvent>.GetEvent("PlayerReady").RemoveListener(OnPlayerReadyInput);
    }

    private void OnPlayerReadyInput(GameObject player)
    {
        if (player.name == lobbyUIEventHandler.assignedPlayerObj.name)
        {
            readyVisual.SetActive(!readyVisual.activeSelf);
        }
    }
}
