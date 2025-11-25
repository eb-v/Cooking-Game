using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HandlePlayerJoining : MonoBehaviour
{
    [SerializeField] private SceneField persistentScene;

    int playerCount = 0;
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerCount++;
        GameObject player = playerInput.gameObject;
        player.name = "Player " + playerCount;
        playerInput.SwitchCurrentActionMap("Lobby");
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").Invoke(player);
        GameManager.Instance.MoveObjectToScene(player, persistentScene);
    }
}
