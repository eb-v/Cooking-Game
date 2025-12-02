using UnityEngine;

[CreateAssetMenu(fileName = "PS_Default", menuName = "Scriptable Objects/States/PlayerState/Default")]
public class DefaultPlayerState : BasePlayerState
{
    private RagdollController _rc;

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
        _rc = gameObject.GetComponent<RagdollController>();
    }


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
