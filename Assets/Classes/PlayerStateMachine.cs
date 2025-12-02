using UnityEngine;

public class PlayerStateMachine
{
    public BasePlayerState currentState { get; private set; }
    public BasePlayerState previousState { get; private set; }

    public void Initialize(BasePlayerState startingState)
    {
        currentState = startingState;
        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    public void Update()
    {
        currentState?.RunUpdateLogic();
    }

    public void FixedUpdate()
    {
        currentState?.RunFixedUpdateLogic();
    }

    public void ChangeState(BasePlayerState newState)
    {
        if (currentState != null)
        {
            previousState = currentState;
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }
        else
        {
            Debug.LogWarning("StateMachine: Attempted to change Player state, but current state is null.");
        }
    }
}
