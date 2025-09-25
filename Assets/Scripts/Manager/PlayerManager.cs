using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    public Transform[] SpawnPoints;
    private int m_playerCount;

    private void Awake()
    {
        
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.transform.position = SpawnPoints[m_playerCount].position;
        m_playerCount++;
        playerInput.name = "Player " + m_playerCount;
    }
}
