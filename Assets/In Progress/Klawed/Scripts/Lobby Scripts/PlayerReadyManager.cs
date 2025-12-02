using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;

public class PlayerReadyManager : MonoBehaviour
{
    [SerializeField] private string assignedChannel = "DefaultChannel";
    [ReadOnly]
    [SerializeField] private UDictionary<GameObject, bool> playerReadyStatus = new UDictionary<GameObject, bool>();

    private static PlayerReadyManager instance;

    [Header("Camera References")]
    [SerializeField] private GameObject lobbyCam;

    public static PlayerReadyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerReadyManager>();
            }
            return instance;
        }
    }




    private void Start()
    {
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").AddListener(OnPlayerJoined);
        GenericEvent<PlayerReadyInputEvent>.GetEvent("PlayerReady").AddListener(OnPlayerReadyInput);
    }

    private void OnPlayerJoined(GameObject player)
    {
        playerReadyStatus[player] = false;
    }

    private void OnPlayerReadyInput(GameObject player)
    {
        bool currentStatus = playerReadyStatus[player];
        playerReadyStatus[player] = !currentStatus;
        if (AllPlayersReady())
        {
            GenericEvent<OnAllPlayersReadyEvent>.GetEvent(assignedChannel).Invoke();
            lobbyCam.SetActive(false);
        }
    }

    private bool AllPlayersReady()
    {
        foreach (var status in playerReadyStatus.Values)
        {
            if (!status)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsPlayerReady(GameObject player)
    {
        return playerReadyStatus[player];
    }

}
