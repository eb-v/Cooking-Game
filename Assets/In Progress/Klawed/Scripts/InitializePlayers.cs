using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InitializePlayers : MonoBehaviour
{
    [SerializeField] private List<Transform> playerSpawnPoints = new List<Transform>();
    [SerializeField] private PlayerManager playerManager;
    private int spawnIndex = 0;

    private void Awake()
    {
        GenericEvent<OnGameStartEvent>.GetEvent("GameStart").AddListener(Initialize);
    }

    
    private void Initialize()
    {
        GameObject playerSpawnPointContainer = GameObject.Find("Player Spawn Points");
        foreach (Transform child in playerSpawnPointContainer.transform)
        {
            playerSpawnPoints.Add(child);
        }
        if (playerSpawnPoints.Count != 4)
        {
            Debug.LogError("There must be exactly 4 player spawn points in the scene.");
            return;
        }

        foreach (GameObject player in playerManager.players)
        {
           // player.SetActive(false);
            player.transform.position = playerSpawnPoints[spawnIndex].position;
            
            player.transform.rotation = Quaternion.identity;
            player.transform.localScale = Vector3.one * 3f;

            Rigidbody rootRb = player.GetComponent<RagdollController>().GetPelvis().GetComponent<Rigidbody>();
            rootRb.isKinematic = false;
            player.GetComponent<RagdollController>().enabled = true;

            spawnIndex++;
            player.SetActive(true);

            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("Player");
            playerInput.SwitchCurrentControlScheme(playerManager.PlayerInputDeviceMappings[playerInput]);
            //InputDevice device = playerManager.PlayerInputDeviceMappings[playerInput];

            //var user = playerInput.user;
            //playerInput.user.ActivateControlScheme("Gamepad");
            //playerInput.user.AssociateActionsWithUser(playerInput.actions);
            //playerInput.user.UnpairDevices();
            //InputUser.PerformPairingWithDevice(device, user: playerInput.user);


            // Ensure action map association is updated
            //playerInput.SwitchCurrentControlScheme(device);


            // do debug statement for player input device assignment
            Debug.Log($"Player {playerInput.name} assigned to device {playerManager.PlayerInputDeviceMappings[playerInput].displayName}");
        }
    }

}
