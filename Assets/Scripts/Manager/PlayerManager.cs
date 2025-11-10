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
    [SerializeField] private List<PlayerLobbySpawnInfoSO> lobbySpawnInfoSOs = new List<PlayerLobbySpawnInfoSO>();
    [SerializeField] private string lobbySceneName = "Pregame Lobby";

    private Dictionary<PlayerInput, InputDevice> playerInputDeviceMappings = new Dictionary<PlayerInput, InputDevice>();
    public Dictionary<PlayerInput, InputDevice> PlayerInputDeviceMappings => playerInputDeviceMappings;

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
        Scene currentScene = SceneManager.GetActiveScene();

        players.Add(playerInput.gameObject);
        playerInputDeviceMappings[playerInput] = playerInput.devices[0];
        Debug.Log($"Player joined: {playerInput.name} with device {playerInput.devices[0].displayName}");
        
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
    }
    
    public int GetPlayerCount() => m_playerCount;
    
    public GameObject GetPlayer1() => players[0];
    public GameObject GetPlayer2() => players[1];
    public GameObject GetPlayer3() => players[2];
    public GameObject GetPlayer4() => players[3];
}