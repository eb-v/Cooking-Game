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
    private static List<GameObject> players = new List<GameObject>();
    public static List<GameObject> Players => players;

    [ReadOnly] [SerializeField] private static int m_playerCount = 0;
    public static int PlayerCount => m_playerCount;

    [SerializeField] private List<PlayerLobbySpawnInfoSO> lobbySpawnInfoSOs = new List<PlayerLobbySpawnInfoSO>();
    [SerializeField] private string lobbySceneName = "Pregame Lobby";
    
    private Dictionary<PlayerInput, InputDevice> playerInputDeviceMappings = new Dictionary<PlayerInput, InputDevice>();
    private HashSet<int> registeredPlayerNumbers = new HashSet<int>();
    
    public Dictionary<PlayerInput, InputDevice> PlayerInputDeviceMappings => playerInputDeviceMappings;

    private void Awake()
    {
        m_playerCount = 0;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").AddListener(OnPlayerJoined);
    }

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        
        if (currentScene.name == lobbySceneName && players.Count > 0)
        {
            ClearAllPlayers();
        }
    }

    public void OnPlayerJoined(GameObject player)
    {
        players.Add(player);
        m_playerCount++;
        //playerInputDeviceMappings[playerInput] = playerInput.devices[0];
        player.name = "Player " + m_playerCount;

        //if (currentScene.name == lobbySceneName)
        //{
        //    Rigidbody rb = playerInput.GetComponent<RagdollController>().GetPelvis().GetComponent<Rigidbody>();
        //    playerInput.GetComponent<RagdollController>().enabled = false;
        //    rb.isKinematic = true;
        //    PlayerLobbySpawnInfoSO spawnInfoSO = lobbySpawnInfoSOs[m_playerCount - 1];
        //    Quaternion spawnRot = Quaternion.Euler(spawnInfoSO.spawnRot);
        //    playerInput.transform.SetPositionAndRotation(spawnInfoSO.spawnPos, spawnRot);
        //    playerInput.transform.parent = gameObject.transform;
        //}
        
    }

    public void ClearAllPlayers()
    {
        foreach (var player in players)
        {
            if (player != null)
                Destroy(player);
        }
        
        players.Clear();
        playerInputDeviceMappings.Clear();
        registeredPlayerNumbers.Clear();
        m_playerCount = 0;
    }
}