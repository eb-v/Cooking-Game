using UnityEngine;

[CreateAssetMenu(fileName = "PS_Unconscious", menuName = "Scriptable Objects/States/PlayerState/Unconscious")]
public class UnconsciousPlayerState : BasePlayerState
{
    private RagdollController _rc;

    public override void Enter()
    {
        base.Enter();
        _rc.EnterLogic();

    }

    public override void Exit()
    {
        base.Exit();
        _rc.ExitLogic();
    }

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
    }

    public override void RunFixedUpdateLogic()
    {
        base.RunFixedUpdateLogic();
        _rc.FixedUpdateLogic();
    }

    public override void RunUpdateLogic()
    {
        base.RunUpdateLogic();
        _rc.UpdateLogic();
    }
}
