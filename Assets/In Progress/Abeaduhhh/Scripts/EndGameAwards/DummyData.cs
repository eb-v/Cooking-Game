using System.Collections.Generic;
using UnityEngine;

public class DummyAwardsData : MonoBehaviour {
    [SerializeField] private EndGameAwards awardsManager;

    private void Start() {
        // Create dummy player stats using GameObjects
        List<PlayerStats> dummyPlayers = new List<PlayerStats>();

        for (int i = 0; i < 4; i++) {
            // Create a new GameObject for this dummy player
            GameObject playerGO = new GameObject($"DummyPlayer_{i + 1}");

            // Add PlayerStats component to the GameObject
            PlayerStats p = playerGO.AddComponent<PlayerStats>();
            p.playerNumber = i + 1;
            p.ingredientsHandled = Random.Range(5, 20);
            p.pointsGenerated = Random.Range(50, 200);
            p.jointsReconnected = Random.Range(0, 10);
            p.explosionsReceived = Random.Range(0, 5);

            dummyPlayers.Add(p);
        }

        // Populate EndGameData with dummy players
        EndGameData.PopulateFromPlayers(dummyPlayers);

        // Show awards
        awardsManager.ShowAwards(this);
    }
}
