using System.Runtime.CompilerServices;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [Header("All Game States")]
    [SerializeField] private BaseStateSO _mainMenuState;
    [SerializeField] private BaseStateSO _lobbyState;
    [SerializeField] private BaseStateSO _inLevelState;
    [SerializeField] private BaseStateSO _endLevelState;


    private StateMachine _stateMachine;

    public BaseStateSO _mainMenuStateInstance;
    public BaseStateSO _lobbyStateInstance;
    public BaseStateSO _inLevelStateInstance;
    public BaseStateSO _endLevelStateInstance;


    private void Awake()
    {
        _stateMachine = new StateMachine();
        _mainMenuStateInstance = Instantiate(_mainMenuState);
        _lobbyStateInstance = Instantiate(_lobbyState);
        _inLevelStateInstance = Instantiate(_inLevelState);
        _endLevelStateInstance = Instantiate(_endLevelState);
    }


    // this should start the game in the main menu state
    private void Start()
    {
        _stateMachine.Initialize(_mainMenuStateInstance);
    }


}
