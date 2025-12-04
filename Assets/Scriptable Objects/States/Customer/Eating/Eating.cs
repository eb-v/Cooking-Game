using UnityEngine;

[CreateAssetMenu(fileName = "CS_Eating", menuName = "Scriptable Objects/States/Customer/Eating")]
public class Eating : CustomerState
{
    private Customer customer;

    private float eatingDuration = 5f;
    private float eatingTimer = 0f;

    public override void Enter()
    {
        base.Enter();
        //customer.Animator.SetBool("isEating", true); 
        customer.SpawnFoodInHand();

        if (customer.currentOrder != null)
        {
            if (customer.currentOrder.GetOrderType() == MenuItemType.Drink) 
            {
                customer.Animator.SetBool("isDrinking", true);
                customer.Animator.SetBool("isEating", false);
            }
            else
            {
                customer.Animator.SetBool("isEating", true);
                customer.Animator.SetBool("isDrinking", false);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        customer.DestroyFoodInHand();
        customer.Animator.SetBool("isEating", false); 
        customer.Animator.SetBool("isDrinking", false); 
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

        eatingTimer += Time.deltaTime;
        if (eatingTimer >= eatingDuration)
        {
            eatingTimer = 0f;
            Debug.Log("Customer finished eating and is leaving.");
            GenericEvent<CustomerFinishedEating>.GetEvent("CustomerFinishedEating").Invoke(customer);
            customer.ChangeState(customer._leaveInstance);
        }
    }
}
