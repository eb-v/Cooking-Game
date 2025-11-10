//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TransformedObject : BaseStation {
//    [SerializeField] private GameObject assembledItemPrefab;
//    [SerializeField] private DeliveryManager deliveryManager;

//    [Header("Remove Pop Force")]
//    [SerializeField] private float verticalForceMultiplier = 8f;
//    [SerializeField] private float horizontalForceMultiplier = 8f;

//    private AssembledItemObject assembledItem;

//    private void Awake()
//    {
//        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(Interact);
//        GenericEvent<RemovePlacedObject>.GetEvent(gameObject.name).AddListener(RemovePlacedKitchenObj);
//        assembledItem = gameObject.AddComponent<AssembledItemObject>();
//    }

//    public override void Interact(GameObject player)
//    {
//        RagdollController ragdollController = player.GetComponent<RagdollController>();

//        if (ragdollController.IsHoldingSomething())
//        {
//            GameObject heldKitchenObj = ragdollController.GetHeldObject();
//            if (heldKitchenObj.CompareTag("Prepared"))
//            {
//                ReleaseFromHands(ragdollController, heldKitchenObj);

//                PlaceOnCounter(player, heldKitchenObj);

//                //add ingredient to this current dish/assembling item
//                //FIX UI OF FINISHED ITEM HERE!!!
//                assembledItem.AddIngredient(heldKitchenObj);
//                Debug.Log("Prepared Ingredient: " + heldKitchenObj.name + " added to AssembledItem");
//            }
//            else 
//            {
//                Debug.Log("Item not a prepared ingredient :(");
//            }
//        }
//        else
//        {
//            Debug.Log("Player not carrying anything");
//        }
//    }

//    private void ReleaseFromHands(RagdollController ragdollController, GameObject heldKitchenObj)
//    {
//        if (ragdollController.leftHand.GetComponent<GrabDetection>().isGrabbing &&
//            ragdollController.leftHand.GetComponent<GrabDetection>().grabbedObj == heldKitchenObj)
//        {
//            GameObject leftArm = ragdollController.leftHand.transform.parent.gameObject;
//            Destroy(leftArm.GetComponent<FixedJoint>());
//            ragdollController.leftHand.GetComponent<GrabDetection>().isGrabbing = false;
//            ragdollController.leftHand.GetComponent<GrabDetection>().grabbedObj = null;
//        }

//        if (ragdollController.rightHand.GetComponent<GrabDetection>().isGrabbing &&
//            ragdollController.rightHand.GetComponent<GrabDetection>().grabbedObj == heldKitchenObj)
//        {
//            GameObject rightArm = ragdollController.rightHand.transform.parent.gameObject;
//            Destroy(rightArm.GetComponent<FixedJoint>());
//            ragdollController.rightHand.GetComponent<GrabDetection>().isGrabbing = false;
//            ragdollController.rightHand.GetComponent<GrabDetection>().grabbedObj = null;
//        }
//    }

//    private void PlaceOnCounter(GameObject player, GameObject heldKitchenObj)
//    {
//        Collider stationCollider = player.GetComponent<Env_Interaction>().currentlyLookingAt.GetComponent<Collider>();

//        float stationYOffset = stationCollider.bounds.extents.y;
//        Vector3 placePos = stationCollider.bounds.center;
//        placePos.y += stationYOffset;

//        heldKitchenObj.transform.position = placePos;
//        heldKitchenObj.transform.rotation = Quaternion.identity;
//        heldKitchenObj.GetComponent<Rigidbody>().isKinematic = true;

//        SetStationObject(heldKitchenObj);
//    }

//    //finalize assembleditem here
//    /*
//    public override void RemovePlacedKitchenObj(GameObject player)
//    {
//        List<GameObject> ingredients = assembledItem.GetIngredients();
//        if (ingredients.Count == 0) return;

//        Vector3 playerPos = player.GetComponent<RagdollController>().centerOfMass.position;
//        Vector3 popDirection = (playerPos - transform.position).normalized;
//        popDirection.y = 0f;
//        Vector3 spawnPos = transform.position + Vector3.up * 1.0f;

//        //spawn new item
//        GameObject newAssembledItem = Instantiate(assembledItemPrefab, spawnPos, Quaternion.identity);
//        newAssembledItem.tag = "AssembledItem";

//        Rigidbody rb = newAssembledItem.GetComponent<Rigidbody>();
//        if (rb != null)
//        {
//            rb.isKinematic = false;
//            rb.AddForce(Vector3.up * verticalForceMultiplier, ForceMode.Impulse);
//            rb.AddForce(popDirection * horizontalForceMultiplier, ForceMode.Impulse);
//        }

//        //new assembledobject added to prefab
//        AssembledItemObject newItemComponent = newAssembledItem.AddComponent<AssembledItemObject>();

//        //copy ingredients from ref to new assembleditemobj
//        foreach (GameObject ingredient in ingredients)
//        {
//            if (ingredient != null)
//            {
//                Rigidbody iRB = ingredient.GetComponent<Rigidbody>();
//                if (iRB != null)
//                {
//                    iRB.isKinematic = true;
//                }

//                Collider col = ingredient.GetComponent<Collider>();
//                if (col != null)
//                    col.enabled = false;

//                ingredient.transform.SetParent(newItemComponent.transform);
//                ingredient.transform.localPosition = Vector3.zero; 
//                ingredient.transform.localRotation = Quaternion.identity;

//                newItemComponent.AddIngredient(ingredient);
//                Debug.Log("Added ingredient to new assembled item: " + ingredient.name);
//            }
//            else
//            {
//                Debug.LogWarning("Tried to add a null ingredient!");
//            }
//        }

//        assembledItem.ClearIngredients();
//        ClearStationObject();

//        Debug.Log("Spawned AssembledItem at: " + spawnPos);
//    }*/

//    public override void RemovePlacedKitchenObj(GameObject player)
//    {
//        List<GameObject> ingredients = assembledItem.GetIngredients();
//        if (ingredients.Count == 0) return;

//        Vector3 spawnPos = transform.position + Vector3.up * 1.0f;

//        // Check for matched recipe
//        RecipeSO matchedRecipe = deliveryManager.GetMatchingRecipeFromAll(assembledItem);
//        GameObject finalPrefab = (matchedRecipe != null) ? matchedRecipe.finalProductPrefab : assembledItemPrefab;

//        // Spawn the final burger/custom item
//        GameObject newBurger = Instantiate(finalPrefab, spawnPos, Quaternion.identity);
//        newBurger.tag = "AssembledItem";

//        //make sure has assembleditemobject
//        AssembledItemObject newItemComponent = newBurger.GetComponent<AssembledItemObject>();
//        if (newItemComponent == null)
//            newItemComponent = newBurger.AddComponent<AssembledItemObject>();

//        // Copy ingredient data
//        foreach (GameObject ingredient in ingredients)
//        {
//            if (ingredient != null)
//            {
//                // Add ingredient to new prefab’s data
//                newItemComponent.AddIngredient(ingredient);

//                //make og invisible and non-interactable
//                ingredient.SetActive(false); // hides it
//                Rigidbody rb = ingredient.GetComponent<Rigidbody>();
//                if (rb != null) rb.isKinematic = true;

//                Collider col = ingredient.GetComponent<Collider>();
//                if (col != null) col.enabled = false;
//            }
//        }

//        //clear transformer internal data
//        assembledItem.ClearIngredients();
//        ClearStationObject();

//        Debug.Log($"Spawned {(matchedRecipe != null ? "final product" : "custom assembled item")} at {spawnPos}");
//    }


//}
