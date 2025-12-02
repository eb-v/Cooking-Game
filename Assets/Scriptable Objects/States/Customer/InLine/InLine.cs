using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "CS_InLine", menuName = "Scriptable Objects/States/Customer/InLine")]
public class InLine : CustomerState
{
    private Customer customer;
    private NavMeshAgent agent;

    public override void Enter()
    {
        base.Enter();
        customer.MoveTo(customer.targetLine.position);
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
        customer = gameObject.GetComponent<Customer>();
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    public override void InteractLogic(GameObject player)
    {
        base.InteractLogic(player);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        WaitInLine();
    }

    private void WaitInLine()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();
            gameObject.transform.rotation = customer.targetLine.rotation;

            if (customer.AtFrontOfLine())
            {
                stateMachine.ChangeState(customer._orderingInstance);
            }
        }
    }
}
