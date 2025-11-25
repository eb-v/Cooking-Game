using UnityEngine;

public class LoadPlayerData
{
    public static PlayerData GetPlayerData()
    {
        PlayerData playerData = Resources.Load<PlayerData>("Data/Player/PlayerData");
        if (playerData == null)
        {
            Debug.LogError("LoadPlayerData: PlayerData asset not found in Resources/Data/Player/PlayerData");
            return null;
        }    
        return playerData;
    }
}
