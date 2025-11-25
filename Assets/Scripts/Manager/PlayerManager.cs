using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerManager : MonoBehaviour
{

    private static PlayerManager instance;

    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerManager>();
            }
            return instance;
        }
    }

    private List<GameObject> players = new List<GameObject>();
    public List<GameObject> Players => players;

    [ReadOnly] [SerializeField] private int m_playerCount = 0;
    public int PlayerCount => players.Count;

    
    private Dictionary<PlayerInput, InputDevice> playerInputDeviceMappings = new Dictionary<PlayerInput, InputDevice>();
    private HashSet<int> registeredPlayerNumbers = new HashSet<int>();
    
    public Dictionary<PlayerInput, InputDevice> PlayerInputDeviceMappings => playerInputDeviceMappings;


    private void OnEnable()
    {
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").AddListener(OnPlayerJoined);
    }

    private void OnDisable()
    {
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").RemoveListener(OnPlayerJoined);
    }


    private void OnPlayerJoined(GameObject player)
    {
        players.Add(player);
    }

  public void MovePlayersToSpawnPositions()
{
    List<Transform> SpawnPoints = GetPlayerSpawnPoints();
    if (SpawnPoints == null)
    {
        Debug.LogError("No spawn points found for players!");
        return;
    }
    if (SpawnPoints.Count < players.Count)
    {
        Debug.LogError("Not enough spawn points for all players!");
        return;
    }
    
    Debug.Log(PlayerCount + " players to move to spawn points.");
    
    for (int i = 0; i < players.Count; i++)
    {
        players[i].transform.position = SpawnPoints[i].position;
        Debug.Log("moved player " + i + " to spawn point " + i);
        
        // CAPTURE the ragdoll ROOT position after spawning
        if (players[i].TryGetComponent<RagdollController>(out RagdollController rc))
        {
            // Wait a frame for physics to settle, then capture
            StartCoroutine(CaptureInitialRagdollPosition(rc, SpawnPoints[i]));
        }
    }
}

private System.Collections.IEnumerator CaptureInitialRagdollPosition(RagdollController rc, Transform spawnPoint)
{
    // Wait a frame for the ragdoll to settle at spawn position
    yield return new WaitForFixedUpdate();
    
    // Capture the ROOT rigidbody position
    if (rc.RagdollDict.ContainsKey(RagdollController.ROOT))
    {
        Transform rootTransform = rc.RagdollDict[RagdollController.ROOT].transform;
        rc.CaptureInitialPosition(rootTransform.position, rootTransform.rotation);
        Debug.Log($"[PlayerManager] Captured ROOT position: {rootTransform.position}");
    }
    else
    {
        // Fallback to parent transform
        rc.CaptureInitialPosition(rc.transform.position, rc.transform.rotation);
        Debug.Log($"[PlayerManager] Captured parent position: {rc.transform.position}");
    }
}
    private List<Transform> GetPlayerSpawnPoints()
    {
        List<Transform> spawnPoints = new List<Transform>();
        GameObject spawnPosContainer = GameObject.Find("Player Spawn Points");
        foreach (Transform child in spawnPosContainer.transform)
        {
            spawnPoints.Add(child);
        }
        return spawnPoints;
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

    public void SwitchPlayerActionMaps(string actionMapName)
    {
        foreach (var player in players)
        {
            if (player.TryGetComponent<PlayerInput>(out PlayerInput playerInput))
            {
                playerInput.SwitchCurrentActionMap(actionMapName);
            }
        }
    }
}