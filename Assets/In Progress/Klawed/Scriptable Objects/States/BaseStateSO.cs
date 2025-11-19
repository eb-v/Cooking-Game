using UnityEngine;

public class BaseStateSO : ScriptableObject
{
    protected GameObject gameObject;
    protected StateMachine stateMachine;

    public virtual void Initialize(GameObject gameObject, StateMachine stateMachine)
    {
        this.gameObject = gameObject;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Execute()
    {

    }

    public virtual void FixedUpdateLogic()
    {
    }

    public virtual void UpdateLogic()
    {
    }

    public virtual void Exit()
    {

    }



}
