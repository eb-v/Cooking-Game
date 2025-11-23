using UnityEngine;

[CreateAssetMenu(fileName = "StoveState_Burning", menuName = "Scriptable Objects/States/Kitchen/Stove/Burning")]
public class StoveBurningState : StoveState {
    private float burnTimer = 0f;
    private GameObject fireInstance;
    public override void Initialize(GameObject gameObject, StateMachine<StoveState> stateMachine) {
        base.Initialize(gameObject, stateMachine);
        stove = gameObject.GetComponent<Stove>();
    }

    public override void Enter() {
        base.Enter();

        burnTimer = 0f;
        stove._stoveUICanvas.enabled = true;
        stove._stoveUIBurnFillImage.enabled = true;
        stove._stoveUICookFillImage.enabled = false;

        Debug.Log("Entered Stove Burning State");

        //if (FireSystem.SystemEnabled) {
        //    var burnable = stove.GetComponent<Burnable>();
        //    if (burnable != null) {
        //        //burnable.TryIgniteWithDelay();
        //    } else {
        //        Debug.LogWarning("Stove has no Burnable!");
        //    }

        //}
    }
    public override void UpdateLogic() {
        base.UpdateLogic();

        burnTimer += Time.deltaTime;


    }
    //private void IgniteStove() {
    //    var burnable = stove.GetComponent<Burnable>();
    //    if (burnable != null && !burnable.IsOnFire) {
    //        burnable.Ignite(); // DIRECT ignition, no spreading delay
    //        Debug.Log("Stove has caught fire from overcooking!");
    //    }
    //}

    public override void Exit() {
        base.Exit();

        burnTimer = 0f;

    }

    //private void BurnIngredient() {
    //    if (stove._currentRecipe == null) {
    //        Debug.LogError("Burning: No recipe while trying to burn ingredient!");
    //        return;
    //    }

    //    GameObject burnedPrefab = stove._currentRecipe.output[^1].Prefab;
    //    GameObject burnedObj = Instantiate(burnedPrefab, stove.objectSnapPoint.position, Quaternion.identity);

    //    var ingredientScript = burnedObj.GetComponent<IngredientScript>();
    //    ingredientScript.grabCollider.enabled = false;

    //    Transform physicsObj = burnedObj.GetComponent<PhysicsTransform>().physicsTransform;
    //    Rigidbody rb = physicsObj.GetComponent<Rigidbody>();

    //    rb.isKinematic = true;
    //    physicsObj.position = stove.objectSnapPoint.position;

    //    Destroy(stove._currentObject);
    //    stove._currentObject = burnedObj;
    //    stove._currentRecipe = null;
    //}
}
