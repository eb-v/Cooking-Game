using UnityEngine;

public class PlayerLobbyPlacementManager : MonoBehaviour
{
    [SerializeField] private Transform[] lobbySpawnPoints;
    private int currentSpawnIndex = 0;

    private void Start()
    {
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").AddListener(OnPlayerJoined);
    }

    private void OnPlayerJoined(GameObject player)
    {
        player.transform.position = lobbySpawnPoints[currentSpawnIndex].position;
        player.transform.rotation = lobbySpawnPoints[currentSpawnIndex].rotation;
        PlayerSystemsManager.TurnOffPlayerMovement(player);
        currentSpawnIndex++;
    }

}
