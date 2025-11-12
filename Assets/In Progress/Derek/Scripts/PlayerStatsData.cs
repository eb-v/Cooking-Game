using System;

[Serializable]
public class PlayerStatsData
{
    public int playerNumber;
    public int pointsGenerated = 0;
    public int jointsReconnected = 0;
    public int explosionsReceived = 0;
    public int itemsGrabbed = 0;

    public PlayerStatsData(int playerNum)
    {
        playerNumber = playerNum;
    }

    public string GetStatsString()
    {
        return $"Player {playerNumber}\nPoints: {pointsGenerated}\nJoints Reconnected: {jointsReconnected}\nExplosions: {explosionsReceived}\nItems Grabbed: {itemsGrabbed}";
    }
}