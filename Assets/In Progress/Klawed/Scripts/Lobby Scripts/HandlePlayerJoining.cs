using UnityEngine;
using UnityEngine.InputSystem;

public class HandlePlayerJoining : MonoBehaviour
{
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        GameObject player = playerInput.gameObject;
        player.name = "Player " + (PlayerManager.PlayerCount);
        playerInput.SwitchCurrentActionMap("Lobby");
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").Invoke(player);
    }
}
