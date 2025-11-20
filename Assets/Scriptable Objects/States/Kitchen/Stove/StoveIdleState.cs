using UnityEngine;

[CreateAssetMenu(fileName = "StoveState_Idle", menuName = "Scriptable Objects/States/Kitchen/Stove/Idle")]
public class StoveIdleState : StoveState
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

    public override void Initialize(GameObject gameObject, StateMachine<StoveState> stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
        stove = gameObject.GetComponent<Stove>();
        if (stove == null)
        {
            Debug.LogError("StoveIdleState: Stove component not found on " + gameObject.name);
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    public override void InteractLogic(GameObject player, Stove stove)
    {
        base.InteractLogic(player, stove);


        GrabScript grabScript = player.GetComponent<GrabScript>();
        if (grabScript.isGrabbing)
        {
            GameObject grabbedObject = grabScript.grabbedObject.GetGameObject();
            if (stove.IsItemCompatible(grabbedObject))
            {
                if (stove.hasObject)
                {
                    Debug.Log("StoveIdleState: Stove already has an object.");
                    return;
                }

                IngredientScript ingredientScript = grabbedObject.GetComponent<IngredientScript>();
                CookingRecipe cookingRecipe = ingredientScript.recipes.Find(recipe => recipe is CookingRecipe) as CookingRecipe;
                stove._currentRecipe = cookingRecipe;
                stove._currentObject = grabbedObject;
                PlaceObjectOntoStove(grabScript.grabbedObject);
                stateMachine.ChangeState(stove._cookingStateInstance);
            }
            else
            {
                Debug.Log("StoveIdleState: The grabbed object is not compatible with the stove.");
            }
        }
        else
        {
            stove.RemoveObject(player);
        }
        

    }

    public override void AltInteractLogic(GameObject player)
    {
        base.AltInteractLogic(player);
    }

    private void PlaceObjectOntoStove(IGrabable grabbedObj)
    {
        stove._currentObject = grabbedObj.GetGameObject();
        IngredientScript ingredient = stove._currentObject.GetComponent<IngredientScript>();
        ingredient.grabCollider.enabled = false;


        grabbedObj.ReleaseObject(grabbedObj.currentPlayer);

        Transform physicsObj = grabbedObj.GetGameObject().GetComponent<PhysicsTransform>().physicsTransform;

        if (physicsObj == null)
        {
            Debug.LogError("The grabbed object does not have a PhysicsTransform component.");
            return;
        }

        physicsObj.transform.position = stove.objectSnapPoint.position;
        physicsObj.transform.rotation = Quaternion.identity;

        Rigidbody rb = physicsObj.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    


}
