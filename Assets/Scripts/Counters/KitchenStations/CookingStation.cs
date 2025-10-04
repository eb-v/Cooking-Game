using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingStation : BaseStation, IPrepStation {
    [Header("Cooking Settings")]
    [SerializeField] private float defaultCookingTime = 5f;
    [SerializeField] private GameObject cookingUI;
    private Slider progressSlider;
    private Image fillImage;
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
    [SerializeField] private FireEffect fireEffect;

    public GameObject currentUser { get; set; }
    private bool _isBeingUsed;
    public bool isBeingUsed { get => _isBeingUsed; set => _isBeingUsed = value; }
    public GameObject currentPlacedObject { get => ingredientInPan ?? panOnStove; set { } }
    public bool containsObject { get => panOnStove != null || ingredientInPan != null; set { } }

    private void Awake() {

        fireEffect.gameObject.SetActive(true);

        if (cookingUI != null) {
            progressSlider = cookingUI.GetComponentInChildren<Slider>();
            if (progressSlider != null) {
                Transform fillTransform = progressSlider.transform.Find("Fill Area/Fill");
                if (fillTransform != null) fillImage = fillTransform.GetComponent<Image>();
            }

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
            if (progressSlider != null) progressSlider.value = Mathf.Clamp01(progress);
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
                fireEffect.gameObject.SetActive(true);
                BurnItem();
            }
        }
    }
    
//checking interactions
    public override void Interact(GameObject player) {
        RagdollController ragdoll = player.GetComponent<RagdollController>();
        if (!ragdoll.IsHoldingSomething()) {
            if (isComplete && ingredientInPan != null) {
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

    public override void RemovePlacedKitchenObj(GameObject player) {
        if ((isComplete || isBurning) && ingredientInPan != null) {
            TakeOutCookedItem();
            return;
        }

        if (ingredientInPan != null && !isCooking) {
            Destroy(ingredientInPan);
            ingredientInPan = null;
            StopCooking();
            return;
        }

        if (panOnStove != null) {
            Rigidbody rb = panOnStove.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
            panOnStove.transform.SetParent(null);
            panOnStove = null;
            StopCooking();
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
            if (progressSlider != null) progressSlider.value = 0f;
            if (fillImage != null) fillImage.color = progressColor;
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
        if (progressSlider != null) progressSlider.value = 0f;
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

        if (progressSlider != null) progressSlider.value = 1f;
        if (fillImage != null) fillImage.color = completeColor;
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

        CookingRecipeSO recipe = GetCookingRecipeWithInput(ingredientInPan);
        Vector3 panPos = panOnStove.transform.position + Vector3.up * 0.1f;

        Destroy(ingredientInPan);

        GameObject burntObj = null;

        // start the fire...
        if (isBurning && fireEffect != null) {
            fireEffect.StartFire();
        }


        if (recipe != null && recipe.canBurn && recipe.burntOutput != null) {
            burntObj = ObjectPoolManager.SpawnObject(recipe.burntOutput, panPos, Quaternion.identity);
        } else {
            burntObj = ObjectPoolManager.SpawnObject(ingredientInPan, panPos, Quaternion.identity);
            burntObj.name = "Burnt " + ingredientInPan.name;
            var renderer = burntObj.GetComponentInChildren<Renderer>();
            if (renderer != null) renderer.material.color = Color.black;
        }

        burntObj.transform.SetParent(panOnStove.transform);
        burntObj.transform.localPosition = Vector3.up * 0.1f;
        burntObj.GetComponent<Rigidbody>().isKinematic = true;

        ingredientInPan = burntObj;

        if (completeText != null) {
            completeText.text = "Burnt!";
            completeText.gameObject.SetActive(true);
        }

        if (fillImage != null) fillImage.color = burnColor;

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

        if (fireEffect != null) {
            fireEffect.StopFire();
        }

        fireEffect.gameObject.SetActive(false);

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
