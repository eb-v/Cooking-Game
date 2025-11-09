using UnityEngine;

[CreateAssetMenu(fileName = "PizzaOvenIdleSO", menuName = "Scriptable Objects/States/Kitchen/PizzaOvenIdleSO")]
public class PizzaOvenIdleSO : BaseStateSO
{
    PizzaOvenScript pizzaOvenScript;

    public override void Initialize(GameObject gameObject, StateMachine _stateMachine)
    {
        base.Initialize(gameObject, _stateMachine);

        pizzaOvenScript = gameObject.GetComponent<PizzaOvenScript>();
        if (pizzaOvenScript == null)
        {
            Debug.LogError("PizzaOvenScript component not found on the GameObject.");
        }
    }

    public override void Enter()
    {
        if (pizzaOvenScript != null)
        {
            pizzaOvenScript.OpenDoor();
        }
    }

    public override void Execute()
    {
        if (pizzaOvenScript.IsClosed() && pizzaOvenScript.pizzaInOven != null)
        {
            stateMachine.ChangeState(pizzaOvenScript._cookingStateInstance);
        }

    }


    public override void Exit()
    {

    }


}
