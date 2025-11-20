using System.Runtime.CompilerServices;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("All Game States")]
    [SerializeField] private GameState _mainMenuState;
    [SerializeField] private GameState _lobbyState;
    [SerializeField] private GameState _preLevelState;
    [SerializeField] private GameState _inLevelState;
    [SerializeField] private GameState _postLevelState;


    private static StateMachine<GameState> _stateMachine;

    #region State Instances
    [HideInInspector] public GameState _mainMenuStateInstance;
    [HideInInspector] public GameState _lobbyStateInstance;
    [HideInInspector] public GameState _preLevelStateInstance;
    [HideInInspector] public GameState _inLevelStateInstance;
    [HideInInspector] public GameState _postLevelStateInstance;
    #endregion

    [Header("Debugging")]
    [ReadOnly]
    [SerializeField] private GameState _currentState;

    public static GameManager Instance { get; private set; }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _stateMachine = new StateMachine<GameState>();
        _mainMenuStateInstance = Instantiate(_mainMenuState);
        _lobbyStateInstance = Instantiate(_lobbyState);
        _preLevelStateInstance = Instantiate(_preLevelState);
        _inLevelStateInstance = Instantiate(_inLevelState);
        _postLevelStateInstance = Instantiate(_postLevelState);

        _mainMenuStateInstance.Initialize(this.gameObject, _stateMachine);
        _lobbyStateInstance.Initialize(this.gameObject, _stateMachine);
        _preLevelStateInstance.Initialize(this.gameObject, _stateMachine);
        _inLevelStateInstance.Initialize(this.gameObject, _stateMachine);
        _postLevelStateInstance.Initialize(this.gameObject, _stateMachine);
    }


    private void Start()
    {
        _stateMachine.Initialize(_mainMenuStateInstance);
        _currentState = _stateMachine.GetCurrentState();
    }


    private void Update()
    {
        _stateMachine.RunUpdateLogic();
    }

    private void FixedUpdate()
    {
        _stateMachine.RunFixedUpdateLogic();
    }

    public GameState GetCurrentState()
    {
        return _stateMachine.GetCurrentState();
    }

    public void ChangeState(GameState newState)
    {
        _stateMachine.ChangeState(newState);
        _currentState = _stateMachine.GetCurrentState();
    }
}
