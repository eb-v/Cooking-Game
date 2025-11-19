using UnityEngine;

public class BasePlayerState : ScriptableObject
{
    protected GameObject gameObject;
    protected PlayerStateMachine stateMachine;

    public virtual void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        this.gameObject = gameObject;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {

    }

    public virtual void RunUpdateLogic()
    {

    }

    public virtual void RunFixedUpdateLogic()
    {

    }

    public virtual void Exit()
    {

    }
}
