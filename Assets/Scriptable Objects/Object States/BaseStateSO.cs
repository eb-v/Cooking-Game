using UnityEngine;

public class BaseStateSO : ScriptableObject
{
    protected GameObject gameObject;

    public virtual void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public virtual void DoEnterLogic() { }
    public virtual void DoUpdateLogic() { }
    public virtual void DoFixedUpdateLogic() { }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void ResetValues() { }
    public virtual void DoAnimationTriggerEventLogic(AnimationTypeEvents type) { }
}
