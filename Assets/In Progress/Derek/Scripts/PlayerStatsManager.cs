using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance;
    
    private List<PlayerStats> allPlayers = new List<PlayerStats>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnEnable()
    {
        // Subscribe to player joined event
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").AddListener(OnPlayerJoined);
    }
    
    private void OnDisable()
    {
        // Unsubscribe from player joined event
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").RemoveListener(OnPlayerJoined);
    }
    
    private void OnPlayerJoined(int playerNumber)
    {
        // Get the player GameObject from PlayerManager
        GameObject playerObj = PlayerManager.Instance.players[playerNumber - 1];
        
        // Check if PlayerStats already exists
        PlayerStats existingStats = playerObj.GetComponent<PlayerStats>();
        if (existingStats != null && allPlayers.Contains(existingStats))
        {
            Debug.LogWarning($"Player {playerNumber} already registered for stats tracking, skipping duplicate");
            return;
        }
        
        // Get or add PlayerStats component
        PlayerStats stats = playerObj.GetComponent<PlayerStats>();
        if (stats == null)
        {
            stats = playerObj.AddComponent<PlayerStats>();
            Debug.Log($"Added new PlayerStats component to Player {playerNumber}");
        }
        
        // Set player number
        stats.playerNumber = playerNumber;
        
        // Add to allPlayers list if not already there
        if (!allPlayers.Contains(stats))
        {
            allPlayers.Add(stats);
            Debug.Log($"Player {playerNumber} registered for stats tracking");
        }
    }
    
    // Method for EndGameAwards or other scripts to get all players
    public List<PlayerStats> GetAllPlayers()
    {
        // Remove any null references (in case players were destroyed)
        allPlayers.RemoveAll(player => player == null);
        return allPlayers;
    }
    
    // Get a specific player's stats by player number
    public PlayerStats GetPlayerStats(int playerNumber)
    {
        return allPlayers.Find(stats => stats.playerNumber == playerNumber);
    }
    
    // Completely clear all tracked players (for returning to main menu, etc.)
    public void ClearAllPlayers()
    {
        allPlayers.Clear();
        Debug.Log("All player stats cleared");
    }
}