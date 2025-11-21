using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "CS_Ordering", menuName = "Scriptable Objects/States/Customer/Ordering")]
public class OrderingState : CustomerState
{
    private Customer customer;

    public override void Enter()
    {
        base.Enter();
        customer.DisplayImage(true);
    }

    public override void Exit()
    {
        base.Exit();
        customer.DisplayImage(false);
    }

    public override void FixedUpdateLogic()
    {
        base.FixedUpdateLogic();
    }

    public override void Initialize(GameObject gameObject, StateMachine<CustomerState> stateMachine)
    {
        base.Initialize(gameObject, stateMachine);

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
                if (menuItemScript.MenuItem == customer.currentOrder)
                {
                    ServeCustomer(heldObject, player);
                }
                else
                {
                    Debug.Log("Customer did not order this item.");
                }
            }
            else
            {
                Debug.Log("Held object is not a MenuItem.");
            }
        }
        else
        {
            Debug.Log("Player is not holding any object.");
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    private void ServeCustomer(GameObject menuItemObject, GameObject player)
    {
        IGrabable grabable = menuItemObject.GetComponent<IGrabable>();
        grabable.ReleaseObject(player);
        Destroy(menuItemObject);
        GenericEvent<OnCustomerServed>.GetEvent("OnCustomerServed").Invoke(customer);
        customer.ChangeState(customer._walkToTableInstance);
        // increase score/money here
    }

}
