using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    //public RawImage[] hudSlots;
    public Transform[] SpawnPoints;
    private int m_playerCount;

    

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.transform.position = SpawnPoints[m_playerCount].position;
        m_playerCount++;
        playerInput.name = "Player " + m_playerCount;
        RagdollController ragdollController = playerInput.gameObject.GetComponent<RagdollController>();

    }
}
