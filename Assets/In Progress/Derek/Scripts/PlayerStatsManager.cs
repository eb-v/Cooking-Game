using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance;

    private Dictionary<int, PlayerStatsData> playerStatsData = new Dictionary<int, PlayerStatsData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Pregame Lobby")
        {
            if (playerStatsData.Count > 0)
            {
                ClearAllPlayers();
            }
        }
    }

    private void OnEnable()
    {
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").AddListener(OnPlayerJoined);
    }

    private void OnDisable()
    {
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").RemoveListener(OnPlayerJoined);
    }

    private void OnPlayerJoined(int playerNumber)
    {
        if (playerStatsData.ContainsKey(playerNumber))
        {
            return;
        }

        PlayerStatsData newStats = new PlayerStatsData(playerNumber);
        playerStatsData[playerNumber] = newStats;
    }

    public void AddPoints(int playerNumber, int points)
    {
        if (playerStatsData.ContainsKey(playerNumber))
        {
            playerStatsData[playerNumber].pointsGenerated += points;
        }
    }

    public void IncrementItemsGrabbed(int playerNumber)
    {
        if (playerStatsData.ContainsKey(playerNumber))
        {
            playerStatsData[playerNumber].itemsGrabbed++;
        }
    }

    public void IncrementJointsReconnected(int playerNumber)
    {
        if (playerStatsData.ContainsKey(playerNumber))
        {
            playerStatsData[playerNumber].jointsReconnected++;
        }
    }

    public void IncrementExplosionsReceived(int playerNumber)
    {
        if (playerStatsData.ContainsKey(playerNumber))
        {
            playerStatsData[playerNumber].explosionsReceived++;
        }
    }

    public List<PlayerStatsData> GetAllPlayersData()
    {
        return new List<PlayerStatsData>(playerStatsData.Values);
    }

    public PlayerStatsData GetPlayerStats(int playerNumber)
    {
        if (playerStatsData.ContainsKey(playerNumber))
        {
            return playerStatsData[playerNumber];
        }
        return null;
    }

    public void ClearAllPlayers()
    {
        playerStatsData.Clear();
    }
}