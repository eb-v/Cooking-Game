using UnityEngine;

public class Player : MonoBehaviour
{
    #region State Bases
    [Header("Player States")]
    [SerializeField] private BasePlayerState _defaultStateBase;
    [SerializeField] private BasePlayerState _airborneStateBase;
    [SerializeField] private BasePlayerState _cuttingStateBase;
    [SerializeField] private BasePlayerState _unconsciousStateBase;
    #endregion


    private PlayerStateMachine _stateMachine;

    #region State Instances
    [HideInInspector] public BasePlayerState _defaultStateInstance { get; private set; }
    [HideInInspector] public BasePlayerState _airborneStateInstance { get; private set; }
    [HideInInspector] public BasePlayerState _cuttingStateInstance { get; private set; }
    [HideInInspector] public BasePlayerState _unconsciousStateInstance { get; private set; }
    #endregion

    private void Awake()
    {
        _stateMachine = new PlayerStateMachine();

        _defaultStateInstance = Instantiate(_defaultStateBase);
        _airborneStateInstance = Instantiate(_airborneStateBase);
        _cuttingStateInstance = Instantiate(_cuttingStateBase);
        _unconsciousStateInstance = Instantiate(_unconsciousStateBase);
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _defaultStateInstance.Initialize(gameObject, _stateMachine);
        _airborneStateInstance.Initialize(gameObject, _stateMachine);
        _cuttingStateInstance.Initialize(gameObject, _stateMachine);
        _unconsciousStateInstance.Initialize(gameObject, _stateMachine);

        _stateMachine.Initialize(_defaultStateInstance);
    }

    public void ChangeState(BasePlayerState newState)
    {
        _stateMachine.ChangeState(newState);
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

}
