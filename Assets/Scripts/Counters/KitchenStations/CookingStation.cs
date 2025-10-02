using UnityEngine;
using UnityEngine.UI;

public class CookingStation : BaseStation, IPrepStation {
    [Header("Cooking Settings")]
    [SerializeField] private float defaultCookingTime = 5f;
    [SerializeField] private GameObject cookingUI;
    private Slider progressSlider;

    private GameObject panOnStove;
    private GameObject ingredientInPan;
    private bool isCooking = false;
    private float cookingTimer = 0f;

    [Header("Recipes")]
    [SerializeField] private CookingRecipeSO[] cookingRecipeSOArray;

    public GameObject currentUser { get; set; }

    private bool _isBeingUsed;
    public bool isBeingUsed {
        get => _isBeingUsed;
        set => _isBeingUsed = value;
    }

    public GameObject currentPlacedObject {
        get => ingredientInPan ?? panOnStove;
        set { /* optional setter, not used */ }
    }

    public bool containsObject {
        get => panOnStove != null || ingredientInPan != null;
        set { /* optional setter, not used */ }
    }

    private void Awake() {
        if (cookingUI != null) {
            progressSlider = cookingUI.GetComponentInChildren<Slider>();
            cookingUI.SetActive(false);
        }

        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(Interact);
        GenericEvent<RemovePlacedObject>.GetEvent(gameObject.name).AddListener(RemovePlacedKitchenObj);
        GenericEvent<AlternateInteractInput>.GetEvent(gameObject.name).AddListener(StartCookingProgress);
        GenericEvent<PlayerStoppedLookingAtInteractable>.GetEvent(gameObject.name).AddListener(StopCooking);
    }

    private void Update() {

        if (isCooking && ingredientInPan != null) {
            Debug.Log("Cooking Started.");
            cookingTimer += Time.deltaTime;

            if (progressSlider != null)
                progressSlider.value = cookingTimer / GetCookingTime(ingredientInPan);

            if (cookingTimer >= GetCookingTime(ingredientInPan))
                FinishCooking();
        }
    }

    public override void Interact(GameObject player) {
        RagdollController ragdoll = player.GetComponent<RagdollController>();
        if (!ragdoll.IsHoldingSomething()) return;

        GameObject heldObj = ragdoll.GetHeldObject();

        if (panOnStove == null && heldObj.CompareTag("Pan")) {
            DetachFromHands(ragdoll, heldObj);
            PlacePanOnStation(player, heldObj);
            return;
        }

        if (panOnStove != null && ingredientInPan == null && HasRecipeWithInput(heldObj)) {
            DetachFromHands(ragdoll, heldObj);
            PlaceIngredientInPan(heldObj);
            return;
        }

        Debug.Log("Cannot place this object here.");
    }

    public void StartCookingProgress(GameObject player) {
        Debug.Log("AlternateInteractInput received!");

        if (panOnStove != null && ingredientInPan != null && !isCooking) {
            currentUser = player;
            isCooking = true;
            _isBeingUsed = true;
            cookingTimer = 0f;
            cookingUI?.SetActive(true);
            Debug.Log("Cooking started!");
        }
    }

    public override void RemovePlacedKitchenObj(GameObject player) {
        if (ingredientInPan != null) {
            Rigidbody rb = ingredientInPan.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            ingredientInPan.transform.SetParent(null);
            ingredientInPan = null;
            StopCooking();
        } else if (panOnStove != null) {
            Rigidbody rb = panOnStove.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            panOnStove.transform.SetParent(null);
            panOnStove = null;
            ingredientInPan = null;
            StopCooking();
        } else {
            Debug.Log("CookingStation has nothing to remove");
        }
    }

    public void PrepIngredient() {
        if (ingredientInPan == null) return;

        CookingRecipeSO recipe = GetCookingRecipeWithInput(ingredientInPan);
        if (recipe == null) return;

        Vector3 pos = ingredientInPan.transform.position;
        Destroy(ingredientInPan);

        GameObject cookedObj = ObjectPoolManager.SpawnObject(recipe.output, pos, Quaternion.identity);
        cookedObj.transform.SetParent(panOnStove.transform);
        cookedObj.transform.localPosition = Vector3.up * 0.1f;
        cookedObj.GetComponent<Rigidbody>().isKinematic = true;

        ingredientInPan = cookedObj;
        Debug.Log("Ingredient prepared!");
    }

    private void StopCooking() {
        if (isCooking) {
            isCooking = false;
            _isBeingUsed = false;
            cookingTimer = 0f;
            cookingUI?.SetActive(false);
            if (progressSlider != null)
                progressSlider.value = 0f;
            currentUser = null;
            Debug.Log("Cooking stopped.");
        }
    }

    private void FinishCooking() {
        CookingRecipeSO recipe = GetCookingRecipeWithInput(ingredientInPan);
        if (recipe == null) return;

        Vector3 pos = ingredientInPan.transform.position;
        Destroy(ingredientInPan);

        GameObject cookedObj = ObjectPoolManager.SpawnObject(recipe.output, pos, Quaternion.identity);
        cookedObj.transform.SetParent(panOnStove.transform);
        cookedObj.transform.localPosition = Vector3.up * 0.1f;
        cookedObj.GetComponent<Rigidbody>().isKinematic = true;

        ingredientInPan = cookedObj;

        StopCooking();
        Debug.Log("Ingredient cooked!");
    }

    private void PlacePanOnStation(GameObject player, GameObject pan) {
        Collider stationCollider = player.GetComponent<Env_Interaction>().currentlyLookingAt.GetComponent<Collider>();
        Vector3 placePos = stationCollider.bounds.center + Vector3.up * stationCollider.bounds.extents.y;

        pan.transform.position = placePos;
        pan.transform.rotation = Quaternion.identity;
        pan.GetComponent<Rigidbody>().isKinematic = true;

        panOnStove = pan;
        SetStationObject(pan);
        Debug.Log("Pan placed on CookingStation.");
    }

    private void PlaceIngredientInPan(GameObject ingredient) {
        ingredient.transform.SetParent(panOnStove.transform);
        ingredient.transform.localPosition = Vector3.up * 0.1f;
        ingredient.transform.localRotation = Quaternion.identity;
        ingredient.GetComponent<Rigidbody>().isKinematic = true;

        ingredientInPan = ingredient;
        Debug.Log("Ingredient placed in pan.");

        StartCookingProgress(currentUser ?? ingredient);
    }

    private bool HasRecipeWithInput(GameObject kitchenObject) {
        return GetCookingRecipeWithInput(kitchenObject) != null;
    }

    private CookingRecipeSO GetCookingRecipeWithInput(GameObject kitchenObject) {
        string objName = kitchenObject.name.Replace("(Clone)", "").Trim();
        foreach (var recipe in cookingRecipeSOArray) {
            if (recipe.input.name == objName) return recipe;
        }
        return null;
    }

    private float GetCookingTime(GameObject ingredient) {
        CookingRecipeSO recipe = GetCookingRecipeWithInput(ingredient);
        return recipe != null && recipe.cookingTime > 0 ? recipe.cookingTime : defaultCookingTime;
    }

    private void DetachFromHands(RagdollController ragdoll, GameObject heldObj) {
        var leftGrab = ragdoll.leftHand.GetComponent<GrabDetection>();
        if (leftGrab.isGrabbing && leftGrab.grabbedObj == heldObj) {
            Destroy(ragdoll.leftHand.transform.parent.GetComponent<FixedJoint>());
            leftGrab.isGrabbing = false;
            leftGrab.grabbedObj = null;
        }

        var rightGrab = ragdoll.rightHand.GetComponent<GrabDetection>();
        if (rightGrab.isGrabbing && rightGrab.grabbedObj == heldObj) {
            Destroy(ragdoll.rightHand.transform.parent.GetComponent<FixedJoint>());
            rightGrab.isGrabbing = false;
            rightGrab.grabbedObj = null;
        }
    }
}
