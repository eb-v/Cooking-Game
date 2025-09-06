using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRunSO", menuName = "Player/State Behavior Logic/Run")]
public class PlayerRunSO : BaseStateSO
{
    private Animator animator;

    public override void Initialize(GameObject gameObject)
    {
        base.Initialize(gameObject);
        animator = gameObject.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        animator.ResetTrigger("Idle");
        animator.SetTrigger("Run");
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFixedUpdateLogic()
    {
        base.DoFixedUpdateLogic();
    }

    public override void DoUpdateLogic()
    {
        base.DoUpdateLogic();
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
    public override void DoAnimationTriggerEventLogic(AnimationTypeEvents type)
    {
        base.DoAnimationTriggerEventLogic(type);
    }

}
