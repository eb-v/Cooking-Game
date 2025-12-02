using UnityEngine;

[CreateAssetMenu(fileName = "PS_Airborne", menuName = "Scriptable Objects/States/PlayerState/Airborne")]
public class AirbornePlayerState : BasePlayerState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
    }

    public override void RunFixedUpdateLogic()
    {
        base.RunFixedUpdateLogic();
    }

    public override void RunUpdateLogic()
    {
        base.RunUpdateLogic();
    }
}
