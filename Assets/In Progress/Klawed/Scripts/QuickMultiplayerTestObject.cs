using UnityEngine;
using UnityEngine.InputSystem;

public class QuickMultiplayerTestObject : MonoBehaviour
{
    private int playercount = 1;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.gameObject.name = "Player_" + playercount;
        playercount++;  
    }
}
