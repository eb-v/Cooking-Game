using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public LobbyUIManager lobbyUIManager;
    public Transform[] SpawnPoints;
    public List<GameObject> players = new List<GameObject>();
    [SerializeField] private int m_playerCount = 0;
    private List<PlayerStats> allPlayers = new List<PlayerStats>();
    [SerializeField] private List<PlayerLobbySpawnInfoSO> lobbySpawnInfoSOs = new List<PlayerLobbySpawnInfoSO>();
    [SerializeField] private string lobbySceneName = "Pregame Lobby";

    private Dictionary<PlayerInput, InputDevice> playerInputDeviceMappings = new Dictionary<PlayerInput, InputDevice>();
    public Dictionary<PlayerInput, InputDevice> PlayerInputDeviceMappings => playerInputDeviceMappings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        PlayerStats existingStats = playerInput.gameObject.GetComponent<PlayerStats>();
        if (existingStats != null && allPlayers.Contains(existingStats))
        {
            Debug.LogWarning($"Player already registered, skipping duplicate: {playerInput.name}");
            return;
        }

        //if (m_playerCount >= SpawnPoints.Length)
        //{
        //    Debug.LogError("Not enough spawn points for all players!");
        //    return;
        //}



        //playerInput.transform.position = SpawnPoints[m_playerCount].position;
        

        players.Add(playerInput.gameObject);
        playerInputDeviceMappings[playerInput] = playerInput.devices[0];
        Debug.Log($"Player joined: {playerInput.name} with device {playerInput.devices[0].displayName}");
        //DontDestroyOnLoad(playerInput.gameObject);
        m_playerCount++;
        playerInput.name = "Player " + m_playerCount;
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").Invoke(m_playerCount);

        if (currentScene.name == lobbySceneName)
        {
            playerInput.SwitchCurrentActionMap("Lobby");
            lobbyUIManager.EnablePlayerCanvas(m_playerCount);
            Rigidbody rb = playerInput.GetComponent<RagdollController>().GetPelvis().GetComponent<Rigidbody>();
            playerInput.GetComponent<RagdollController>().enabled = false;
            rb.isKinematic = true; 
            PlayerLobbySpawnInfoSO spawnInfoSO = lobbySpawnInfoSOs[m_playerCount - 1];
            Quaternion spawnRot = Quaternion.Euler(spawnInfoSO.spawnRot);
            playerInput.transform.SetPositionAndRotation(spawnInfoSO.spawnPos, spawnRot);
            playerInput.transform.parent = gameObject.transform;
        }
        



            // Get or add PlayerStats component
            PlayerStats stats = playerInput.gameObject.GetComponent<PlayerStats>();
        if (stats == null)
        {
            stats = playerInput.gameObject.AddComponent<PlayerStats>();
           // Debug.Log($"Added new PlayerStats component to Player {m_playerCount}");
        }
        
        // Set player number
        stats.playerNumber = m_playerCount;
        
        // Add to allPlayers list if not already there
        if (!allPlayers.Contains(stats))
        {
            allPlayers.Add(stats);
            // Debug.Log($"Player {m_playerCount} registered for awards tracking");
        }
        
    }
    
    // Method needed by EndGameAwards to get all players
    public List<PlayerStats> GetAllPlayers()
    {
        // Remove any null references (in case players were destroyed)
        allPlayers.RemoveAll(player => player == null);
        return allPlayers;
    }
    
    // Optional: Reset stats between games
    public void ResetAllStats()
    {
        allPlayers.Clear();
        m_playerCount = 0;
    }
    
    public GameObject GetPlayer1() => players[0];
    public GameObject GetPlayer2() => players[1];
    public GameObject GetPlayer3() => players[2];
    public GameObject GetPlayer4() => players[3];
}