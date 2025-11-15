using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class StateMachine
{
    private BaseStateSO currentState;
    private BaseStateSO previousState;

    public void Initialize(BaseStateSO startingState)
    {
        currentState = startingState;
        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    public void ChangeState(BaseStateSO newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
            previousState = currentState;
            currentState = newState;
            currentState.Enter();
        }
        else
        {
            Debug.LogWarning("StateMachine: Attempted to change state, but current state is null.");
        }
    }

    public void RunCurrentStateLogic()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }

    public BaseStateSO GetCurrentState()
    {
        return currentState;
    }



}
