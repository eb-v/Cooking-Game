using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "CS_Ordering", menuName = "Scriptable Objects/States/Customer/Ordering")]
public class OrderingState : CustomerState
{
    private NavMeshAgent agent;
    private Customer customer;

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
        agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on the Customer GameObject.");
        }

        customer = gameObject.GetComponent<Customer>();
        if (customer == null)
        {
            Debug.LogError("Customer component not found on the Customer GameObject.");
        }
    }

    public override void InteractLogic(GameObject player)
    {
        base.InteractLogic(player);

        GrabScript gs = player.GetComponent<GrabScript>();

        if (gs.isGrabbing)
        {
            GameObject heldObject = gs.grabbedObject.GetGameObject();
            MenuItemScript menuItemScript = heldObject.GetComponent<MenuItemScript>();

            if (menuItemScript != null)
            {
                if (menuItemScript.MenuItem == customer.CustomersOrder)
                {
                    ServeCustomer(heldObject, player);
                }
            }
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        WalkToLine();
    }

    private void WalkToLine()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();
            gameObject.transform.rotation = customer.targetLine.rotation;
        }
    }

    private void ServeCustomer(GameObject menuItemObject, GameObject player)
    {
        IGrabable grabable = menuItemObject.GetComponent<IGrabable>();
        grabable.ReleaseObject(player);
        Destroy(menuItemObject);
        customer.ChangeState(customer._walkToTableInstance);
        // increase score/money here
    }

}
