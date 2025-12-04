using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem.Users;


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

    [SerializeField] private GameObject playerPrefab;


    private Dictionary<int, InputDevice> playerIndexDeviceMappings = new Dictionary<int, InputDevice>();
    private HashSet<int> registeredPlayerNumbers = new HashSet<int>();
    
    public Dictionary<int, InputDevice> PlayerIndexDeviceMappings => playerIndexDeviceMappings;


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
        if (player.TryGetComponent<PlayerInput>(out PlayerInput playerInput))
        {
            if (playerInput.devices.Count > 0)
                playerIndexDeviceMappings[playerInput.playerIndex] = playerInput.devices[0];
            else
                Debug.LogWarning($"Player {playerInput.playerIndex} has no devices assigned on join.");
        }
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
        playerIndexDeviceMappings.Clear();
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

    public void RespawnPlayer(GameObject playerToDestroy)
    {
        if (playerToDestroy == null)
        {
            Debug.LogError("Cannot respawn a null player.");
            return;
        }
        PlayerInput destroyedInput = playerToDestroy.GetComponent<PlayerInput>();
        if (destroyedInput == null)
        {
            Debug.LogError("Player object does not have a PlayerInput component.");
            return;
        }

        int playerIndex = destroyedInput.playerIndex;
        string controlScheme = destroyedInput.currentControlScheme;
        string actionMapName = destroyedInput.currentActionMap.name;

        if (!playerIndexDeviceMappings.TryGetValue(playerIndex, out InputDevice deviceToReassign))
        {
            Debug.LogError($"Cannot find saved device for Player Index {playerIndex}. Aborting respawn.");
            return;
        }

        Player playerComponentToDestroy = playerToDestroy.GetComponent<Player>();
        playerComponentToDestroy.SavePlayerCustomization();

        Material[] savedMaterials = playerComponentToDestroy.Materials;
        Sprite savedFace = playerComponentToDestroy.FaceSprite;

        players.Remove(playerToDestroy);
        GenericEvent<PlayerDestroy>.GetEvent(playerToDestroy.GetInstanceID().ToString()).Invoke();
        Destroy(playerToDestroy);

        PlayerInput newPlayerInput = PlayerInput.Instantiate(
        playerPrefab,
        controlScheme: controlScheme,
        playerIndex: playerIndex,
        pairWithDevices: new InputDevice[] { deviceToReassign } // <-- Use the array overload
        );

        InputUser.PerformPairingWithDevice(deviceToReassign, newPlayerInput.user);

        players.Add(newPlayerInput.gameObject);



        List<Transform> spawnPoints = GetPlayerSpawnPoints();
        if (spawnPoints.Count > playerIndex)
        {
            newPlayerInput.gameObject.transform.position = spawnPoints[playerIndex].position;
        }


        GameObject newPlayerObj = newPlayerInput.gameObject;
        Player newPlayer = newPlayerObj.GetComponent<Player>();
        newPlayer.LoadPlayerCustomization(savedMaterials, savedFace);

        Debug.Log($"Player {playerIndex} successfully respawned and rebound to device: {deviceToReassign.displayName}");
    }

    private void SaveCosmetics()
    {

    }


}