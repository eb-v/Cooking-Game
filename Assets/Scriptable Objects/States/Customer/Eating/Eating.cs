using UnityEngine;

[CreateAssetMenu(fileName = "CS_Eating", menuName = "Scriptable Objects/States/Customer/Eating")]
public class Eating : CustomerState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdateLogic()
    {
        base.FixedUpdateLogic();
    }

    public override void Initialize(GameObject gameObject, StateMachine<CustomerState> stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
    }

    public override void InteractLogic(GameObject player)
    {
        base.InteractLogic(player);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }
}
