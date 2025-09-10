using UnityEngine;

[CreateAssetMenu(fileName = "PlayerJumpSO", menuName = "Player/State Behavior Logic/Jump")]
public class PlayerJumpSO : BaseStateSO
{
    private Animator animator;

    public override void DoAnimationTriggerEventLogic(AnimationTypeEvents type)
    {
        base.DoAnimationTriggerEventLogic(type);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        animator.SetTrigger("Jump");
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

    public override void Initialize(GameObject gameObject)
    {
        base.Initialize(gameObject);
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    public override void ResetValues()
    {
        base.ResetValues();
        animator.ResetTrigger("Jump");
    }
}
