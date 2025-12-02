using System;
using UnityEngine;

[Serializable]
public class PlayerStatsData
{
    public GameObject player;
    public int pointsGenerated = 0;
    public int jointsReconnected = 0;
    public int explosionsReceived = 0;
    public int itemsGrabbed = 0;

    public PlayerStatsData(GameObject player)
    {
        this.player = player;
    }

    public string GetStatsString()
    {
        return $"Player {player.name}\nPoints: {pointsGenerated}\nJoints Reconnected: {jointsReconnected}\nExplosions: {explosionsReceived}\nItems Grabbed: {itemsGrabbed}";
    }
}