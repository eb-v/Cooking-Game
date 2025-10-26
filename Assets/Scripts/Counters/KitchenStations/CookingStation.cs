using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingStation : BaseStation, IPrepStation {
    [Header("Cooking Settings")]
    [SerializeField] private float defaultCookingTime = 5f;
    [SerializeField] private GameObject cookingUI;
    //[SerializeField] private Image progressSlider;
    [SerializeField] private Image fillImage;
    private TextMeshProUGUI completeText;

    [Header("Progress Colors")]
    [SerializeField] private Color progressColor = Color.green;
    [SerializeField] private Color completeColor = Color.yellow;
    [SerializeField] private Color burnColor = Color.red;

    [Header("Cooking Objects")]
    private GameObject panOnStove;
    private GameObject ingredientInPan;
    private bool isCooking = false;
    private bool isComplete = false;
    private bool isBurning = false;
    private float cookingTimer = 0f;
    private float burnTimer = 0f;

    [Header("Burn Settings")]
    [SerializeField] private float burnDelay = 10f;

    [Header("Recipes")]
    [SerializeField] private CookingRecipeSO[] cookingRecipeSOArray;

    [Header("Fire Effects")]
    [SerializeField] private FireController fireEffectPrefab;
    private FireController activeFire;

    public GameObject currentUser { get; set; }
    private bool _isBeingUsed;
    public bool isBeingUsed { get => _isBeingUsed; set => _isBeingUsed = value; }
    public GameObject currentPlacedObject { get => ingredientInPan ?? panOnStove; set { } }
    public bool containsObject { get => panOnStove != null || ingredientInPan != null; set { } }

    private void Awake() {

       // fireEffect.gameObject.SetActive(true);

        if (cookingUI != null) {
            //if (progressSlider != null) {
            //    Transform fillTransform = progressSlider.transform.Find("Fill Area/Fill");
            //    if (fillTransform != null) fillImage = fillTransform.GetComponent<Image>();
            //}

            completeText = cookingUI.GetComponentInChildren<TextMeshProUGUI>();
            if (completeText != null) completeText.gameObject.SetActive(false);

            cookingUI.SetActive(false);
        }

        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(Interact);
        GenericEvent<RemovePlacedObject>.GetEvent(gameObject.name).AddListener(RemovePlacedKitchenObj);
        GenericEvent<AlternateInteractInput>.GetEvent(gameObject.name).AddListener(RemovePlacedKitchenObj);
    }

    private void Update() {
        if (isCooking && ingredientInPan != null) {
            cookingTimer += Time.deltaTime;
            float progress = cookingTimer / GetCookingTime(ingredientInPan);
            if (fillImage != null) fillImage.fillAmount = Mathf.Clamp01(progress);
            if (fillImage != null) fillImage.color = progressColor;

            if (cookingTimer >= GetCookingTime(ingredientInPan)) {
                FinishCooking();
            }
        }

        if (isComplete && !isBurning && ingredientInPan != null) {
            burnTimer += Time.deltaTime;

            float timeLeft = burnDelay - burnTimer;

            if (timeLeft <= 5f && fillImage != null) {
                float t = Mathf.PingPong(Time.time * 4f, 1f);
                fillImage.color = Color.Lerp(progressColor, completeColor, t);
            }

            if (burnTimer >= burnDelay) {
               // fireEffect.gameObject.SetActive(true);
                BurnItem();
            }
        }
    }

    //checking interactions
    public override void Interact(GameObject player) {
        if (isCooking) {
            Debug.Log("Cooking in progress — cannot interact now.");
            return;
        }

        RagdollController ragdoll = player.GetComponent<RagdollController>();

        if (!ragdoll.IsHoldingSomething()) {
            if ((isComplete || isBurning) && ingredientInPan != null) {
                TakeOutCookedItem();
                return;
            }
            return;
        }

        GameObject heldObj = ragdoll.GetHeldObject();

        if (panOnStove == null && heldObj.CompareTag("Pan")) {
            DetachFromHands(ragdoll, heldObj);
            PlacePanOnStation(heldObj);
            return;
        }

        if (panOnStove != null && ingredientInPan == null && HasRecipeWithInput(heldObj)) {
            DetachFromHands(ragdoll, heldObj);
            PlaceIngredientInPan(heldObj);
            return;
        }

        Debug.Log("Cannot place this object here.");
    }

    //can not remove items when the item is raw -> cooked, they can only remove when its cooked -> burnt
    public override void RemovePlacedKitchenObj(GameObject player) {
        if (isCooking) {
            Debug.Log("You can’t remove anything while cooking!");
            return;
        }

        if ((isComplete || isBurning) && ingredientInPan != null) {
            TakeOutCookedItem();
            return;
        }

        if (ingredientInPan != null && !isCooking && !isComplete && !isBurning) {
            Destroy(ingredientInPan);
            ingredientInPan = null;
            StopCooking();
            Debug.Log("Raw ingredient removed from pan.");
            return;
        }

        if (panOnStove != null && ingredientInPan == null) {
            Rigidbody rb = panOnStove.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
            panOnStove.transform.SetParent(null);
            panOnStove = null;
            StopCooking();
            Debug.Log("Pan removed from stove.");
        }
    }



    //cooking idea, gets player input to start cooking, starts the progress bar,
    //allows for the player to take it out when cooked, if not taken out within 10 seconds, the item burns and starts the fire.
    private void StartCooking() {
        if (panOnStove != null && ingredientInPan != null && !isCooking) {
            isCooking = true;
            _isBeingUsed = true;
            isComplete = false;
            isBurning = false;
            cookingTimer = 0f;
            burnTimer = 0f;

            cookingUI?.SetActive(true);
            if (fillImage != null) fillImage.fillAmount = 0f;
            if (completeText != null) completeText.gameObject.SetActive(false);

            Debug.Log("Cooking started!");
        }
    }

    private void StopCooking() {
        isCooking = false;
        _isBeingUsed = false;
        isComplete = false;
        isBurning = false;
        cookingTimer = 0f;
        burnTimer = 0f;

        cookingUI?.SetActive(false);
        if (fillImage != null) fillImage.fillAmount = 0f;
        if (completeText != null) completeText.gameObject.SetActive(false);
    }

    private void FinishCooking() {
        if (ingredientInPan == null) return;

        isCooking = false;
        isComplete = true;
        burnTimer = 0f;

        CookingRecipeSO recipe = GetCookingRecipeWithInput(ingredientInPan);
        if (recipe != null && recipe.output != null) {
            Vector3 pos = panOnStove.transform.position + Vector3.up * 0.1f;
            Destroy(ingredientInPan);

            GameObject cookedObj = ObjectPoolManager.SpawnObject(recipe.output, pos, Quaternion.identity);
            cookedObj.transform.SetParent(panOnStove.transform);
            cookedObj.transform.localPosition = Vector3.up * 0.1f;
            cookedObj.GetComponent<Rigidbody>().isKinematic = true;
            ingredientInPan = cookedObj;
        }

        if (fillImage != null) fillImage.fillAmount = 1f;
        if (completeText != null) {
            completeText.text = "Complete!";
            completeText.gameObject.SetActive(true);
        }

        Debug.Log("Cooking finished");
    }

    private void BurnItem() {
        if (ingredientInPan == null || panOnStove == null) return;

        isBurning = true;
        isComplete = false;

        Vector3 panPos = panOnStove.transform.position + Vector3.up * 0.1f;

        // Spawn burnt version first
        CookingRecipeSO recipe = GetCookingRecipeWithInput(ingredientInPan);

        GameObject burntObj;

        if (recipe != null && recipe.canBurn && recipe.burntOutput != null)
            burntObj = ObjectPoolManager.SpawnObject(recipe.burntOutput, panPos, Quaternion.identity);
        else {
            burntObj = ObjectPoolManager.SpawnObject(ingredientInPan, panPos, Quaternion.identity);
            burntObj.name = "Burnt " + ingredientInPan.name;
            var renderer = burntObj.GetComponentInChildren<Renderer>();
            if (renderer != null)
                renderer.material.color = Color.black;
        }

        // Remove original ingredient after spawning burnt version
        Destroy(ingredientInPan);
        burntObj.transform.SetParent(panOnStove.transform);
        burntObj.transform.localPosition = Vector3.up * 0.1f;
        burntObj.GetComponent<Rigidbody>().isKinematic = true;
        ingredientInPan = burntObj;

        // Always instantiate a new fire prefab — don’t reuse old activeFire
        if (fireEffectPrefab != null) {
            Debug.Log("Spawning fire effect.");
            FireController fire = ObjectPoolManager
                .SpawnObject(fireEffectPrefab.gameObject, panPos, Quaternion.identity)
                .GetComponent<FireController>();

            fire.ResetFire();
            fire.StartFire();
            Debug.Log("Fire effect started.");
        }

        if (completeText != null) {
            completeText.text = "Burnt!";
            completeText.gameObject.SetActive(true);
        }

        if (fillImage != null)
            fillImage.color = burnColor;

        Debug.Log("Item has burnt!");
    }

    private void TakeOutCookedItem() {
        if (ingredientInPan == null) return;

        isComplete = false;
        isBurning = false;
        burnTimer = 0f;

        Vector3 liftPos = panOnStove.transform.position + Vector3.up * 0.3f;
        ingredientInPan.transform.SetParent(null);
        ingredientInPan.transform.position = liftPos;

        Rigidbody rb = ingredientInPan.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        cookingUI?.SetActive(false);
        ingredientInPan = null;

        _isBeingUsed = false;
        isCooking = false;

        if (activeFire != null) {
            activeFire.StopFire();

            if (activeFire.gameObject != null)
                Destroy(activeFire.gameObject, 1f);

            activeFire = null;
        }

        Debug.Log("Item set above pan");
    }
//helpers to place pan and ingredients for the process
    private void PlacePanOnStation(GameObject pan) {
        Collider stationCollider = GetComponent<Collider>();
        Vector3 placePos = stationCollider.bounds.center + Vector3.up * stationCollider.bounds.extents.y;

        pan.transform.SetParent(null);
        pan.transform.position = placePos;
        pan.transform.rotation = Quaternion.identity;

        Rigidbody rb = pan.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

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
        StartCooking();
    }

    public void PrepIngredient() {
    }

    private bool HasRecipeWithInput(GameObject obj) => GetCookingRecipeWithInput(obj) != null;

    private CookingRecipeSO GetCookingRecipeWithInput(GameObject obj) {
        string nameTrimmed = obj.name.Replace("(Clone)", "").Trim();
        foreach (var recipe in cookingRecipeSOArray) {
            if (recipe.input.name == nameTrimmed) return recipe;
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
