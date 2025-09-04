using UnityEngine;
using System.Collections.Generic;

public class ObjectStateMachine
{
    public BaseStateSO currentState;
    public Dictionary<System.Type, BaseStateSO> stateMap = new Dictionary<System.Type, BaseStateSO>();

    public ObjectStateMachine(Dictionary<System.Type, BaseStateSO> stateMap)
    {
        this.stateMap = stateMap;
    }

    public void Initialize(BaseStateSO startingState)
    {
        currentState = startingState;
        currentState.DoEnterLogic();
    }

    public void ChangeState(BaseStateSO newState)
    {
        currentState.DoExitLogic();
        currentState = newState;
        currentState.DoEnterLogic();
    }
}
