using System.Runtime.CompilerServices;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("All Game States")]
    [SerializeField] private BaseStateSO _mainMenuState;
    [SerializeField] private BaseStateSO _lobbyState;
    [SerializeField] private BaseStateSO _inLevelState;
    [SerializeField] private BaseStateSO _endLevelState;
    [SerializeField] private BaseStateSO _pickModifiersState;


    private StateMachine _stateMachine;

    public BaseStateSO _mainMenuStateInstance;
    public BaseStateSO _lobbyStateInstance;
    public BaseStateSO _inLevelStateInstance;
    public BaseStateSO _endLevelStateInstance;
    public BaseStateSO _pickModifiersStateInstance;

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
        //_mainMenuStateInstance = Instantiate(_mainMenuState);
        //_lobbyStateInstance = Instantiate(_lobbyState);
        _inLevelStateInstance = Instantiate(_inLevelState);
        //_endLevelStateInstance = Instantiate(_endLevelState);
        _pickModifiersStateInstance = Instantiate(_pickModifiersState);

        //_mainMenuStateInstance.Initialize(this.gameObject, _stateMachine);
        //_lobbyStateInstance.Initialize(this.gameObject, _stateMachine);
        _inLevelStateInstance.Initialize(this.gameObject, _stateMachine);
        //_endLevelStateInstance.Initialize(this.gameObject, _stateMachine);
        _pickModifiersStateInstance.Initialize(this.gameObject, _stateMachine);
    }


    private void Start()
    {
        _stateMachine.Initialize(_pickModifiersStateInstance);
    }


    private void Update()
    {
       _stateMachine.RunCurrentStateLogic();
    }


    public void ChangeState(BaseStateSO newState)
    {
        _stateMachine.ChangeState(newState);
    }

    private void ResetVariables()
    {
        _mainMenuStateInstance = null;
        _lobbyStateInstance = null;
        _inLevelStateInstance = null;
        _endLevelStateInstance = null;
        _pickModifiersStateInstance = null;
    }

}
