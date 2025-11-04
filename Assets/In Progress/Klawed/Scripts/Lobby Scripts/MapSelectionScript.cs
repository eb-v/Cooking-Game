using UnityEngine;
using UnityEngine.InputSystem;

public class MapSelectionScript : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerInput playerOneInput;
    [SerializeField] private InputAction nextOptionAction;
    [SerializeField] private InputAction previousOptionAction;
    [SerializeField] private int maxIndex = 1;
    [SerializeField] private int minIndex = -1;
    [SerializeField] private SpringAPI springAPI;
    [SerializeField] private int currentIndex = 0;

    private void Start()
    {
        playerOneInput = playerManager.GetPlayer1().GetComponent<PlayerInput>();
        if (playerOneInput == null)
        {
            Debug.LogError("Player 1 Input not found in MapSelectionScript Start");
            return;
        }
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
            nextOptionAction = lobbyActionMap.FindAction("NextOption");
            previousOptionAction = lobbyActionMap.FindAction("PreviousOption");

        }
    }

    private void OnEnable()
    {
        if (nextOptionAction == null || previousOptionAction == null)
        {
            Debug.LogError("Input Actions not found in MapSelectionScript OnEnable");
            return;
        }
        nextOptionAction.performed += OnNextOption;
        previousOptionAction.performed += OnPreviousOption;

        nextOptionAction?.Enable();
        previousOptionAction?.Enable();

    }

    private void OnDisable()
    {
        if (nextOptionAction == null || previousOptionAction == null)
        {
            Debug.LogError("Input Actions not found in MapSelectionScript OnDisable");
            return;
        }
        nextOptionAction.performed -= OnNextOption;
        previousOptionAction.performed -= OnPreviousOption;

        nextOptionAction?.Disable();
        previousOptionAction?.Disable();
    }

    private void OnNextOption(InputAction.CallbackContext context)
    {
        IncrementIndex();
    }

    private void OnPreviousOption(InputAction.CallbackContext context)
    {
        DecrementIndex();
    }

    private void IncrementIndex()
    {
        currentIndex++;
        if (currentIndex > maxIndex)
        {
            currentIndex = minIndex;
        }
        springAPI.SetGoalValue(currentIndex);
    }

    private void DecrementIndex()
    {
        currentIndex--;
        if (currentIndex < minIndex)
        {
            currentIndex = maxIndex;
        }
        springAPI.SetGoalValue(currentIndex);
    }


}
