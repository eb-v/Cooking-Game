using UnityEngine;

public abstract class BaseStateSO<T> : ScriptableObject where T : BaseStateSO<T>
{
    protected GameObject gameObject;
    protected StateMachine<T> stateMachine;

    public virtual void Initialize(GameObject gameObject, StateMachine<T> stateMachine)
    {
        this.gameObject = gameObject;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void FixedUpdateLogic() { }
    public virtual void UpdateLogic() { }
    public virtual void Exit() { }
}

