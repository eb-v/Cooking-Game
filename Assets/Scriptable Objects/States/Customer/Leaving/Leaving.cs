using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "CS_Leaving", menuName = "Scriptable Objects/States/Customer/Leaving")]
public class Leaving : CustomerState
{
    private Customer customer;
    private NavMeshAgent agent => customer.Agent;

    public override void Enter()
    {
        base.Enter();
        customer.MoveTo(customer.exitPoint.position);
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
            Debug.Log("Customer has exited the restaurant.");
            Destroy(customer.gameObject);
        }
    }

}
