using System.Collections.Generic;
using UnityEngine;

public class DummyDataInitializer : MonoBehaviour {
    [Header("Dummy Player Prefab")]
    [SerializeField] private GameObject playerVisualPrefab;

    [Header("Number of Test Players")]
    [SerializeField] private int numPlayers = 4;

    private void Awake() {
        // --- Create dummy EndGameData ---
        EndGameData.playerCount = numPlayers;
        EndGameData.playerObjects = new GameObject[numPlayers];
        EndGameData.ingredientsHandled = new int[numPlayers];
        EndGameData.pointsGenerated = new int[numPlayers];
        EndGameData.jointsReconnected = new int[numPlayers];
        EndGameData.explosionsReceived = new int[numPlayers];

        // --- Populate with fake values ---
        for (int i = 0; i < numPlayers; i++) {
            // Randomized stats for testing variety
            EndGameData.ingredientsHandled[i] = Random.Range(5, 30);
            EndGameData.pointsGenerated[i] = Random.Range(100, 1000);
            EndGameData.jointsReconnected[i] = Random.Range(0, 10);
            EndGameData.explosionsReceived[i] = Random.Range(0, 5);

            // Spawn visual dummy player prefabs
            if (playerVisualPrefab != null) {
                GameObject dummy = Instantiate(playerVisualPrefab);
                dummy.name = $"Dummy Player {i + 1}";
                dummy.SetActive(false); // Hide initially (EndGameAwards will clone it)
                EndGameData.playerObjects[i] = dummy;
            }
        }

        Debug.Log($"[DummyDataInitializer] Created {numPlayers} dummy players for EndGameAwards testing.");
    }

    private void Start() {
        // Automatically show the awards when the scene starts
        if (EndGameAwards.Instance != null) {
            Debug.Log("[DummyDataInitializer] Starting EndGameAwards.ShowAwards()");
            EndGameAwards.Instance.ShowAwards();
        } else {
            Debug.LogWarning("[DummyDataInitializer] No EndGameAwards instance found in scene.");
        }
    }
}
