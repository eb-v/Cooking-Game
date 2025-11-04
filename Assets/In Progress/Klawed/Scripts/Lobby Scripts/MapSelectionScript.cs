using UnityEngine;
using UnityEngine.InputSystem;

public class MapSelectionScript : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    private PlayerInput playerOneInput;
    private InputAction navigateAction;
    private InputAction selectAction;

    private void Awake()
    {
        playerOneInput = playerManager.GetPlayer1().GetComponent<PlayerInput>();
        Initialize();
    }

    private void Initialize()
    {
        if (playerOneInput == null)
        {
            Debug.LogError("Player 1 Input not found in MapSelectionScript");
        }
        else
        {
            var lobbyActionMap = playerOneInput.actions.FindActionMap("Lobby");
            if (lobbyActionMap != null)
            {
                navigateAction = lobbyActionMap.FindAction("Navigate");
                if (navigateAction == null)
                {
                    Debug.LogError("Navigate action not found in Lobby action map");
                }
                selectAction = lobbyActionMap.FindAction("Select");
                if (selectAction == null)
                {
                    Debug.LogError("Select action not found in Lobby action map");
                }
            }
            else
            {
                Debug.LogError("Lobby action map not found in Player 1 Input");
            }
              
        }
    }

    private void OnEnable()
    {
        playerOneInput?.actions
    }
}
