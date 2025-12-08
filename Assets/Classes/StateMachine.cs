using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class StateMachine<T> where T : BaseStateSO<T>
{
    private T currentState;
    private T previousState;

    public void Initialize(T startingState)
    {
        currentState = startingState;
        currentState?.Enter();
    }

    public void ChangeState(T newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        previousState = currentState;
        currentState = newState;
        currentState?.Enter();
    }

    public void RunUpdateLogic()
    {
        currentState?.UpdateLogic();
    }

    public void RunFixedUpdateLogic()
    {
        currentState?.FixedUpdateLogic();
    }

    public T GetCurrentState()
    {
        return currentState;
    }

    public TS GetCurrentState<TS>() where TS : T
    {
        return currentState as TS;
    }
}
