using System.Runtime.CompilerServices;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("All Game States")]
    [SerializeField] private BaseStateSO _mainMenuState;
    [SerializeField] private BaseStateSO _lobbyState;
    [SerializeField] private BaseStateSO _inLevelState;
    [SerializeField] private BaseStateSO _displayEndgameStats;
    [SerializeField] private BaseStateSO _slotMachineState;


    private StateMachine _stateMachine;


    [HideInInspector] public BaseStateSO _mainMenuStateInstance;
    [HideInInspector] public BaseStateSO _lobbyStateInstance;
    [HideInInspector] public BaseStateSO _inLevelStateInstance;
    [HideInInspector] public BaseStateSO _DisplayEndgameStatsInstance;
    [HideInInspector] public BaseStateSO _slotMachineStateInstance;


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

        _stateMachine = new StateMachine();
        _mainMenuStateInstance = Instantiate(_mainMenuState);
        _lobbyStateInstance = Instantiate(_lobbyState);
        _inLevelStateInstance = Instantiate(_inLevelState);
        _DisplayEndgameStatsInstance = Instantiate(_displayEndgameStats);
        _slotMachineStateInstance = Instantiate(_slotMachineState);

        _mainMenuStateInstance.Initialize(this.gameObject, _stateMachine);
        _lobbyStateInstance.Initialize(this.gameObject, _stateMachine);
        _inLevelStateInstance.Initialize(this.gameObject, _stateMachine);
        _DisplayEndgameStatsInstance.Initialize(this.gameObject, _stateMachine);
        _slotMachineStateInstance.Initialize(this.gameObject, _stateMachine);
    }


    private void Start()
    {
        _stateMachine.Initialize(_lobbyStateInstance);
    }


    private void Update()
    {
       _stateMachine.RunCurrentStateLogic();
    }


    public void ChangeState(BaseStateSO newState)
    {
        _stateMachine.ChangeState(newState);
    }


}
