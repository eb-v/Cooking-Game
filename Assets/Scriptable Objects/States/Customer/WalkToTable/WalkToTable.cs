using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "CS_WalkToTable", menuName = "Scriptable Objects/States/Customer/WalkToTable")]
public class WalkToTable : CustomerState
{
    private Customer customer;
    private NavMeshAgent agent => customer.Agent;

    public override void Enter()
    {
        base.Enter();
        customer.MoveTo(customer.assignedTable.position);
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
    }

    public override void InteractLogic(GameObject player)
    {
        base.InteractLogic(player);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();
            gameObject.transform.rotation = customer.assignedTable.rotation;
            customer.ChangeState(customer._eatAtTableInstance);
        }
    }

}
