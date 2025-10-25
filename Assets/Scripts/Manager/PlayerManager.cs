using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    
    //public RawImage[] hudSlots;
    public Transform[] SpawnPoints;
    private int m_playerCount;
    
    // Track all players for stats
    private List<PlayerStats> allPlayers = new List<PlayerStats>();
    
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
    
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        // Check if this player already exists (prevent duplicates from instantiated copies)
        PlayerStats existingStats = playerInput.gameObject.GetComponent<PlayerStats>();
        if (existingStats != null && allPlayers.Contains(existingStats))
        {
            Debug.LogWarning($"Player already registered, skipping duplicate: {playerInput.name}");
            return;
        }
        
        // Make sure we have enough spawn points
        if (m_playerCount >= SpawnPoints.Length)
        {
            Debug.LogError("Not enough spawn points for all players!");
            return;
        }
        
        playerInput.transform.position = SpawnPoints[m_playerCount].position;
        m_playerCount++;
        playerInput.name = "Player " + m_playerCount;
        
        RagdollController ragdollController = playerInput.gameObject.GetComponent<RagdollController>();
        // Comment this out if you don't need pre-game lobby
        // ragdollController.TurnMovementOff();
        
        // Get or add PlayerStats component (but DON'T replace existing one)
        PlayerStats stats = playerInput.gameObject.GetComponent<PlayerStats>();
        if (stats == null)
        {
            stats = playerInput.gameObject.AddComponent<PlayerStats>();
            Debug.Log($"Added new PlayerStats component to Player {m_playerCount}");
        }
        else
        {
            Debug.Log($"Using existing PlayerStats component for Player {m_playerCount}");
        }
        
        // Set player number (this won't reset the stats)
        stats.playerNumber = m_playerCount;
        
        // Make sure this player isn't already in the list
        if (!allPlayers.Contains(stats))
        {
            allPlayers.Add(stats);
            Debug.Log($"Player {m_playerCount} registered. Stats - Ingredients: {stats.ingredientsHandled}, Points: {stats.pointsGenerated}, Revives: {stats.teammatesRevived}");
        }
        
        Debug.Log($"Player {m_playerCount} joined. Total players: {allPlayers.Count}");
    }
    
    public List<PlayerStats> GetAllPlayers()
    {
        // Remove any null references (in case players were destroyed)
        allPlayers.RemoveAll(player => player == null);
        return allPlayers;
    }
    
    public void ResetAllStats()
    {
        allPlayers.Clear();
        m_playerCount = 0;
    }
    
    // Award Winners
    public List<PlayerStats> GetHotHandsWinners()
    {
        // Clean up null references first
        allPlayers.RemoveAll(player => player == null);
        
        if (allPlayers.Count == 0)
        {
            Debug.LogWarning("No players found for Hot Hands award");
            return new List<PlayerStats>();
        }
        
        // Debug all player stats
        Debug.Log("=== Hot Hands Award ===");
        foreach (var player in allPlayers)
        {
            Debug.Log($"Player {player.playerNumber}: {player.ingredientsHandled} ingredients");
        }
        
        int maxIngredients = 0;
        foreach (var player in allPlayers)
        {
            if (player != null && player.ingredientsHandled > maxIngredients)
                maxIngredients = player.ingredientsHandled;
        }
        
        List<PlayerStats> winners = new List<PlayerStats>();
        foreach (var player in allPlayers)
        {
            if (player != null && player.ingredientsHandled == maxIngredients)
                winners.Add(player);
        }
        
        Debug.Log($"Hot Hands Winner(s): {winners.Count} player(s) with {maxIngredients} ingredients");
        return winners;
    }
    
    public List<PlayerStats> GetMVPWinners()
    {
        // Clean up null references first
        allPlayers.RemoveAll(player => player == null);
        
        if (allPlayers.Count == 0)
        {
            Debug.LogWarning("No players found for MVP award");
            return new List<PlayerStats>();
        }
        
        // Debug all player stats
        Debug.Log("=== MVP Award ===");
        foreach (var player in allPlayers)
        {
            Debug.Log($"Player {player.playerNumber}: {player.pointsGenerated} points");
        }
        
        int maxPoints = 0;
        foreach (var player in allPlayers)
        {
            if (player != null && player.pointsGenerated > maxPoints)
                maxPoints = player.pointsGenerated;
        }
        
        List<PlayerStats> winners = new List<PlayerStats>();
        foreach (var player in allPlayers)
        {
            if (player != null && player.pointsGenerated == maxPoints)
                winners.Add(player);
        }
        
        Debug.Log($"MVP Winner(s): {winners.Count} player(s) with {maxPoints} points");
        return winners;
    }
    
    public List<PlayerStats> GetGuardianAngelWinners()
    {
        // Clean up null references first
        allPlayers.RemoveAll(player => player == null);
        
        if (allPlayers.Count == 0) return new List<PlayerStats>();
        
        int maxRevives = 0;
        foreach (var player in allPlayers)
        {
            if (player != null && player.teammatesRevived > maxRevives)
                maxRevives = player.teammatesRevived;
        }
        
        if (maxRevives == 0)
        {
            Debug.Log("No revives this game - skipping Guardian Angel award");
            return new List<PlayerStats>();
        }
        
        // Debug all player stats
        Debug.Log("=== Guardian Angel Award ===");
        foreach (var player in allPlayers)
        {
            Debug.Log($"Player {player.playerNumber}: {player.teammatesRevived} revives");
        }
        
        List<PlayerStats> winners = new List<PlayerStats>();
        foreach (var player in allPlayers)
        {
            if (player != null && player.teammatesRevived == maxRevives)
                winners.Add(player);
        }
        
        Debug.Log($"Guardian Angel Winner(s): {winners.Count} player(s) with {maxRevives} revives");
        return winners;
    }
    
    public List<PlayerStats> GetNoobWinners()
    {
        // Clean up null references first
        allPlayers.RemoveAll(player => player == null);
        
        if (allPlayers.Count == 0)
        {
            Debug.LogWarning("No players found for Noob award");
            return new List<PlayerStats>();
        }
        
        // Debug all player stats
        Debug.Log("=== Noob Award ===");
        foreach (var player in allPlayers)
        {
            Debug.Log($"Player {player.playerNumber}: {player.pointsGenerated} points");
        }
        
        int minPoints = int.MaxValue;
        foreach (var player in allPlayers)
        {
            if (player != null && player.pointsGenerated < minPoints)
                minPoints = player.pointsGenerated;
        }
        
        List<PlayerStats> winners = new List<PlayerStats>();
        foreach (var player in allPlayers)
        {
            if (player != null && player.pointsGenerated == minPoints)
                winners.Add(player);
        }
        
        Debug.Log($"Noob Winner(s): {winners.Count} player(s) with {minPoints} points");
        return winners;
    }
}