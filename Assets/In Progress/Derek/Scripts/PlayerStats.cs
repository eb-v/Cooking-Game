using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerNumber;

    public int pointsGenerated = 0;
    public int jointsReconnected = 0;  // Changed from teammatesRevived
    public int explosionsReceived = 0;
    public int itemsGrabbed = 0;  // NEW: Track items grabbed
    
    // NEW: Call this whenever player grabs any item
    public void IncrementItemsGrabbed()
    {
        itemsGrabbed++;
        Debug.Log($"Player {playerNumber} - Items Grabbed: {itemsGrabbed}");
    }
    
    // Call this whenever player generates points (delivers dish)
    public void AddPoints(int points)
    {
        pointsGenerated += points;
        Debug.Log($"Player {playerNumber} - Total Points: {pointsGenerated}");
    }
    
    // Call this when player reconnects a joint to another player
    public void IncrementJointsReconnected()
    {
        jointsReconnected++;
        Debug.Log($"Player {playerNumber} - Joints Reconnected: {jointsReconnected}");
    }
    
    // Call this when player receives an explosion
    public void IncrementExplosionsReceived()
    {
        explosionsReceived++;
        Debug.Log($"Player {playerNumber} - Explosions Received: {explosionsReceived}");
    }
    
    // Get reference to this player's character/prefab
    public GameObject GetPlayerPrefab()
    {
        return gameObject;
    }
    
    // Get all stats as a formatted string (useful for debugging)
    public string GetStatsString()
    {
        return $"Player {playerNumber}\nPoints: {pointsGenerated}\nJoints Reconnected: {jointsReconnected}\nExplosions: {explosionsReceived}\nItems Grabbed: {itemsGrabbed}";
    }
}