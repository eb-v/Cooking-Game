using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatsManager : MonoBehaviour
{
    private static Dictionary<GameObject, PlayerStatsData> playerStatsData = new Dictionary<GameObject, PlayerStatsData>();
    private static bool isTrackingEnabled = false;

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "Pregame Lobby" || currentScene.Contains("Level"))
        {
            if (playerStatsData.Count > 0)
            {
                ClearAllPlayers();
            }
        }
        
        // NEW: Enable tracking when Level 1 or Level 2 is loaded
        if (currentScene == "Level 1" || currentScene == "Level 2")
        {
            EnableTracking();
        }
        else
        {
            DisableTracking();
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

    private static void OnPlayerJoined(GameObject player)
    {
        if (playerStatsData.ContainsKey(player))
        {
            return;
        }
        PlayerStatsData newStats = new PlayerStatsData(player);
        playerStatsData[player] = newStats;
    }

    public static void EnableTracking()
    {
        isTrackingEnabled = true;
        Debug.Log("Player stats tracking ENABLED");
    }

    public static void DisableTracking()
    {
        isTrackingEnabled = false;
        Debug.Log("Player stats tracking DISABLED");
    }

    public static void AddPoints(GameObject playerNumber, int points)
    {
        if (!isTrackingEnabled) return;
        if (playerStatsData.ContainsKey(playerNumber))
        {
            playerStatsData[playerNumber].pointsGenerated += points;
        }
    }

    public static void IncrementItemsGrabbed(GameObject player)
    {
        if (!isTrackingEnabled) return;
        if (playerStatsData.ContainsKey(player))
        {
            playerStatsData[player].itemsGrabbed++;
        }
    }

    public static void IncrementJointsReconnected(GameObject player)
    {
        if (!isTrackingEnabled) return;
        if (playerStatsData.ContainsKey(player))
        {
            playerStatsData[player].jointsReconnected++;
        }
    }

    public static void IncrementExplosionsReceived(GameObject player)
    {
        if (!isTrackingEnabled) return;
        if (playerStatsData.ContainsKey(player))
        {
            playerStatsData[player].explosionsReceived++;
        }
    }

    public static List<PlayerStatsData> GetAllPlayersData()
    {
        return new List<PlayerStatsData>(playerStatsData.Values);
    }

    public static PlayerStatsData GetPlayerStats(GameObject player)
    {
        if (playerStatsData.ContainsKey(player))
        {
            return playerStatsData[player];
        }
        return null;
    }

    public static void ClearAllPlayers()
    {
        playerStatsData.Clear();
    }
}