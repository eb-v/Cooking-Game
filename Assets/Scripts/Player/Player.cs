using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

    #region State Machine Variables

    public ObjectStateMachine stateMachine { get; private set; }
    [SerializeField] private BaseStateSO initialState;
    #endregion

    #region Scriptable Object Variables
    [SerializeField] private BaseStateSO[] availableStates;
    #endregion

    public Dictionary<System.Type, BaseStateSO> stateInstanceMap;

    private void Awake()
    {
        stateInstanceMap = new Dictionary<System.Type, BaseStateSO>();
        stateMachine = new ObjectStateMachine(stateInstanceMap);

        foreach (BaseStateSO state in availableStates)
        {
            BaseStateSO stateInstance = Instantiate(state);
            stateInstance.Initialize(gameObject);
            stateInstanceMap.Add(state.GetType(), stateInstance);
        }

        GenericEvent<Move>.GetEvent(gameObject.GetInstanceID()).AddListener(ChangeToRun);
        GenericEvent<Idle>.GetEvent(gameObject.GetInstanceID()).AddListener(ChangeToIdle);
    }

    public void Start()
    {
        stateMachine.Initialize(stateInstanceMap[initialState.GetType()]);
    }

    public void OnEnable()
    {
        
    }

    public void OnDisable()
    {

    }

    private void Update()
    {
        stateMachine.currentState.DoUpdateLogic();

        System.Type currentStateType = stateMachine.currentState.GetType();
        if (currentStateType == typeof(PlayerIdleSO))
        {

        }   

        // state Transition Logic
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.DoFixedUpdateLogic();
    }

    public void AnimationTriggerEvent(AnimationTypeEvents type)
    {
        stateMachine.currentState.DoAnimationTriggerEventLogic(type);
    }

    private void ChangeToRun()
    {
        stateMachine.ChangeState(stateInstanceMap[typeof(PlayerRunSO)]);
    }

    private void ChangeToIdle()
    {
        stateMachine.ChangeState(stateInstanceMap[typeof(PlayerIdleSO)]);
    }
}



