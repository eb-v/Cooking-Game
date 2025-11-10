using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLobbySpawnInfoSO", menuName = "Scriptable Objects/Lobby/PlayerLobbySpawnInfoSO")]
public class PlayerLobbySpawnInfoSO : ScriptableObject
{
    public Vector3 spawnPos;
    public Vector3 spawnRot;
}
