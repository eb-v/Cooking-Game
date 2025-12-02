using System.Collections.Generic;
using UnityEngine; // Needed for GameObject

public static class EndGameData {
    public static int playerCount;

    // NEW FIELD: Stores the actual player GameObjects (or their visual copies/prefabs)
    // from the previous scene, which the EndGameAwards script can clone.
    public static GameObject[] playerObjects;

    public static int[] itemsGrabbed;
    public static int[] pointsGenerated;
    public static int[] jointsReconnected;
    public static int[] explosionsReceived;

    public static void PopulateFromPlayers(List<PlayerStats> players, bool skipObjects = false) {
        playerCount = players.Count;

        if (!skipObjects) playerObjects = new GameObject[playerCount];

        itemsGrabbed = new int[playerCount];
        pointsGenerated = new int[playerCount];
        jointsReconnected = new int[playerCount];
        explosionsReceived = new int[playerCount];

        for (int i = 0; i < playerCount; i++) {
            if (!skipObjects) playerObjects[i] = players[i].gameObject; // only if real players exist
            itemsGrabbed[i] = players[i].itemsGrabbed;
            pointsGenerated[i] = players[i].pointsGenerated;
            jointsReconnected[i] = players[i].jointsReconnected;
            explosionsReceived[i] = players[i].explosionsReceived;
        }
    }


}