using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "StoveState_Cooking", menuName = "Scriptable Objects/States/Kitchen/Stove/Cooking")]
public class StoveCookingState : StoveState
{
    private float cookingTimer = 0f;
    private bool isCooked = false;

    public override void AltInteractLogic(GameObject player)
    {
        base.AltInteractLogic(player);
    }

    public override void Enter()
    {
        base.Enter();
        stove._stoveUICanvas.enabled = true;
        stove._stoveUICookFillImage.enabled = true;
        Debug.Log("Entered Stove Cooking State");
    }

    public override void Exit()
    {
        base.Exit();
        cookingTimer = 0f;
        isCooked = false;
        stove._stoveUICookFillImage.enabled = false;
        stove._stoveUIBurnFillImage.enabled = false;
        stove._stoveUICanvas.enabled = false;
    }

    public override void FixedUpdateLogic()
    {
        base.FixedUpdateLogic();
    }

    public override void Initialize(GameObject gameObject, StateMachine<StoveState> stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
        stove = gameObject.GetComponent<Stove>();
    }

    public override void InteractLogic(GameObject player, Stove stove)
    {
        base.InteractLogic(player, stove);
        stove.RemoveObject(player);
        stateMachine.ChangeState(stove._idleStateInstance);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        cookingTimer += Time.deltaTime;
        

        if (!isCooked)
        {
            // cooking logic
            Image fillImage = stove._stoveUICookFillImage;
            fillImage.fillAmount = cookingTimer / stove.cookingDuration;

            if (cookingTimer >= stove.cookingDuration)
            {
                GameObject cookedIngredient = CookIngredient();

                IngredientScript ingredientScript = cookedIngredient.GetComponent<IngredientScript>();
                CookingRecipe cookingRecipe = ingredientScript.recipes.Find(recipe => recipe is CookingRecipe) as CookingRecipe;
                if (cookingRecipe == null)
                {
                    Debug.Log("Ingredient has been cooked and has no further forms. Returning to Idle State.");
                    stateMachine.ChangeState(stove._idleStateInstance);
                }
                else
                {
                    stove._currentRecipe = cookingRecipe;
                    Debug.Log("Ingredient cooked. Continuing in Cooking State for next stage.");
                    cookingTimer = 0f; // Reset timer for next cooking stage
                    isCooked = true; // Mark as cooked to start burn timer
                    stove._stoveUICookFillImage.enabled = false;
                    stove._stoveUIBurnFillImage.enabled = true;
                }

            }
        }
        else
        {
            // burn logic
            Image burnFillImage = stove._stoveUIBurnFillImage;
            burnFillImage.fillAmount = cookingTimer / stove.burnDuration;
            if (cookingTimer >= stove.burnDuration)
            {
                CookIngredient();
                Debug.Log("Ingredient has burned. Going into the burning state.");
                stateMachine.ChangeState(stove._burningStateInstance);

            }

        }

    }

    // Replace the current ingredient on the stove with its cooked version
    private GameObject CookIngredient()
    {
        GameObject cookedObjVersion = Instantiate(stove._currentRecipe.output[0].Prefab, stove.objectSnapPoint.position, Quaternion.identity);
        IngredientScript ingredientScript = cookedObjVersion.GetComponent<IngredientScript>();
        ingredientScript.grabCollider.enabled = false;


        Transform physicsObj = cookedObjVersion.GetComponent<PhysicsTransform>().physicsTransform;
        if (physicsObj == null)
        {
            Debug.LogError("The cooked object does not have a PhysicsTransform component.");
            return null;
        }

        physicsObj.transform.position = stove.objectSnapPoint.position;
        physicsObj.transform.rotation = Quaternion.identity;
        Rigidbody rb = physicsObj.GetComponent<Rigidbody>();
        rb.isKinematic = true;



        Destroy(stove._currentObject);
        stove._currentObject = cookedObjVersion;
        return cookedObjVersion;
    }
}
