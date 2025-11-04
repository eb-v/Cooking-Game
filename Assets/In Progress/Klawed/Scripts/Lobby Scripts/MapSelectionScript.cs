using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapSelectionScript : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerInput playerOneInput;
    [SerializeField] private int maxIndex = 1;
    [SerializeField] private int minIndex = -1;
    [SerializeField] private SpringAPI springAPI;
    [SerializeField] private int currentIndex = 0;
    [SerializeField] private string assignedPlayerName = "Player 1";

    private void Awake()
    {
        GenericEvent<OnNextOptionInput>.GetEvent(assignedPlayerName).AddListener(OnNextOption);
        GenericEvent<OnPreviousOptionInput>.GetEvent(assignedPlayerName).AddListener(OnPreviousOption);
    }


    private void Start()
    {
        //playerOneInput = GameObject.Find("Player 1").GetComponent<PlayerInput>();
        //if (playerOneInput == null)
        //{
        //    Debug.LogError("Player 1 Input not found in MapSelectionScript Start");
        //    return;
        //}
        Initialize();
    }

    private void Initialize()
    {
        //if (playerOneInput == null)
        //{
        //    Debug.LogError("Player 1 Input not found in MapSelectionScript");
        //}
        //else
        //{
        //    var lobbyActionMap = playerOneInput.actions.FindActionMap("Lobby");
        //    nextOptionAction = lobbyActionMap.FindAction("NextOption");
        //    previousOptionAction = lobbyActionMap.FindAction("PreviousOption");
        //    navigateAction = lobbyActionMap.FindAction("Navigate");

        //}
    }

    

   

    private void OnNextOption()
    {
        if (gameObject.activeInHierarchy == false) return;
        IncrementIndex();
    }

    private void OnPreviousOption()
    {
        if (gameObject.activeInHierarchy == false) return;
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
