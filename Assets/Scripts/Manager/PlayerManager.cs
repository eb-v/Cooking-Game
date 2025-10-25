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
}