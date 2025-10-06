using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformedObject : BaseStation {
    [SerializeField] private GameObject assembledItemPrefab;
    [Header("Remove Pop Force")]
    [SerializeField] private float verticalForceMultiplier = 8f;
    [SerializeField] private float horizontalForceMultiplier = 8f;

    private AssembledItemObject assembledItem;

    private void Awake()
    {
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(Interact);
        GenericEvent<RemovePlacedObject>.GetEvent(gameObject.name).AddListener(RemovePlacedKitchenObj);

        assembledItem = new AssembledItemObject();
    }

    public override void Interact(GameObject player)
    {
        RagdollController ragdollController = player.GetComponent<RagdollController>();

        if (ragdollController.IsHoldingSomething())
        {
            GameObject heldKitchenObj = ragdollController.GetHeldObject();
            if (heldKitchenObj.CompareTag("Prepared"))
            {
                ReleaseFromHands(ragdollController, heldKitchenObj);

                PlaceOnCounter(player, heldKitchenObj);

                //add ingredient to this current dish/assembling item
                //FIX UI OF FINISHED ITEM HERE!!!
                assembledItem.AddIngredient(heldKitchenObj);
                Debug.Log("Prepared Ingredient: " + heldKitchenObj.name + " added to AssembledItem");
            }
            else 
            {
                Debug.Log("Item not a prepared ingredient :(");
            }
        }
        else
        {
            Debug.Log("Player not carrying anything");
        }
    }

    private void ReleaseFromHands(RagdollController ragdollController, GameObject heldKitchenObj)
    {
        if (ragdollController.leftHand.GetComponent<GrabDetection>().isGrabbing &&
            ragdollController.leftHand.GetComponent<GrabDetection>().grabbedObj == heldKitchenObj)
        {
            GameObject leftArm = ragdollController.leftHand.transform.parent.gameObject;
            Destroy(leftArm.GetComponent<FixedJoint>());
            ragdollController.leftHand.GetComponent<GrabDetection>().isGrabbing = false;
            ragdollController.leftHand.GetComponent<GrabDetection>().grabbedObj = null;
        }

        if (ragdollController.rightHand.GetComponent<GrabDetection>().isGrabbing &&
            ragdollController.rightHand.GetComponent<GrabDetection>().grabbedObj == heldKitchenObj)
        {
            GameObject rightArm = ragdollController.rightHand.transform.parent.gameObject;
            Destroy(rightArm.GetComponent<FixedJoint>());
            ragdollController.rightHand.GetComponent<GrabDetection>().isGrabbing = false;
            ragdollController.rightHand.GetComponent<GrabDetection>().grabbedObj = null;
        }
    }

    private void PlaceOnCounter(GameObject player, GameObject heldKitchenObj)
    {
        Collider stationCollider = player.GetComponent<Env_Interaction>().currentlyLookingAt.GetComponent<Collider>();

        float stationYOffset = stationCollider.bounds.extents.y;
        Vector3 placePos = stationCollider.bounds.center;
        placePos.y += stationYOffset;

        heldKitchenObj.transform.position = placePos;
        heldKitchenObj.transform.rotation = Quaternion.identity;
        heldKitchenObj.GetComponent<Rigidbody>().isKinematic = true;

        SetStationObject(heldKitchenObj);
    }

    //finalize assembleditem here
    public override void RemovePlacedKitchenObj(GameObject player)
    {
        if (assembledItem.GetIngredients().Count > 0)
        {   
            Vector3 playerPos = player.GetComponent<RagdollController>().centerOfMass.position;
            Vector3 popDirection = (playerPos - transform.position).normalized;
            popDirection.y = 0f;

            Vector3 spawnPos = transform.position + Vector3.up * 3.0f; 
            GameObject newAssembledItem = Instantiate(assembledItemPrefab, spawnPos, Quaternion.identity);

            newAssembledItem.tag = "AssembledItem";

            Rigidbody rb = newAssembledItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(Vector3.up * verticalForceMultiplier, ForceMode.Impulse);
                rb.AddForce(popDirection * horizontalForceMultiplier, ForceMode.Impulse);
            }

            AssembledItemObject assembledItemComponent = new AssembledItemObject();
            foreach (GameObject ingredient in assembledItem.GetIngredients())
            {
                assembledItemComponent.AddIngredient(ingredient);
            }

            AssembledItemContainer container = newAssembledItem.GetComponent<AssembledItemContainer>();
            if (container == null)
                container = newAssembledItem.AddComponent<AssembledItemContainer>();
            container.SetAssembledItemObject(assembledItemComponent);

            foreach (GameObject ingredient in assembledItem.GetIngredients())
            {
                if (ingredient != null)
                    Destroy(ingredient);
            }

            assembledItem = new AssembledItemObject();
            ClearStationObject();

            //check assembleditemcontainer
            if (container != null)
            {
                Debug.Log("Ingredients in new AssembledItem:");
                AssembledItemObject itemObj = container.GetAssembledItemObject();
                if (itemObj != null)
                {
                    foreach (GameObject ingredient in itemObj.GetIngredients())
                    {
                        if (ingredient != null)
                            Debug.Log(ingredient.name + ", ");
                    }
                }
                else
                {
                    Debug.LogWarning("No AssembledItemObject found in container!");
                }
            }
            else
            {
                Debug.LogWarning("No AssembledItemContainer found on the new assembled item!");
            }


            Debug.Log("Spawned AssembledItem at: " + spawnPos);
            Debug.Log("Rigidbody mass: " + rb.mass);
        }
    }
}
