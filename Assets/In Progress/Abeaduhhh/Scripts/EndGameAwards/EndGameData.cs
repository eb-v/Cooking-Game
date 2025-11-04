using System.Collections.Generic;
using UnityEngine; // Needed for GameObject

public static class EndGameData {
    public static int playerCount;

    // NEW FIELD: Stores the actual player GameObjects (or their visual copies/prefabs)
    // from the previous scene, which the EndGameAwards script can clone.
    public static GameObject[] playerObjects;

    public static int[] ingredientsHandled;
    public static int[] pointsGenerated;
    public static int[] jointsReconnected;
    public static int[] explosionsReceived;

    public static void PopulateFromPlayers(List<PlayerStats> players) {
        playerCount = players.Count; // Use static playerCount
        playerObjects = new GameObject[playerCount];

        ingredientsHandled = new int[playerCount];
        pointsGenerated = new int[playerCount];
        jointsReconnected = new int[playerCount];
        explosionsReceived = new int[playerCount];

        for (int i = 0; i < playerCount; i++) {
            // Store the actual player GameObject
            playerObjects[i] = players[i].gameObject;

            // Copy their stats
            ingredientsHandled[i] = players[i].ingredientsHandled;
            pointsGenerated[i] = players[i].pointsGenerated;
            jointsReconnected[i] = players[i].jointsReconnected;
            explosionsReceived[i] = players[i].explosionsReceived;
        }
    }

}