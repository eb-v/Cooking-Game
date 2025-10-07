using UnityEngine;

public class DeliveryStation : BaseStation {
    [SerializeField] private DeliveryManager deliveryManager; 
    [Header("Remove Pop Force")]
    [SerializeField] private float verticalForceMultiplier = 8f;
    [SerializeField] private float horizontalForceMultiplier = 8f;

    private void Awake()
    {
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(Interact);
        GenericEvent<RemovePlacedObject>.GetEvent(gameObject.name).AddListener(RemovePlacedKitchenObj);
    }

    public override void Interact(GameObject player)
    {
        RagdollController ragdollController = player.GetComponent<RagdollController>();

        // if player is carrying something
        if (ragdollController.IsHoldingSomething())
        {
            // if station is empty
            if (!HasKitchenObject())
            {
                GameObject heldKitchenObj = ragdollController.GetHeldObject();
                Debug.Log("Held object: " + heldKitchenObj.name 
                + ", tag: " + heldKitchenObj.tag 
                + ", has AssembledItemObject? " 
                + (heldKitchenObj.GetComponent<AssembledItemObject>() != null));

                AssembledItemObject assembledObj = heldKitchenObj.GetComponentInParent<AssembledItemObject>();
                if (assembledObj != null)
                {
                    heldKitchenObj = assembledObj.gameObject;
                    Debug.Log("Found root AssembledItemObject: " + heldKitchenObj.name);
                }

                if (heldKitchenObj.CompareTag("AssembledItem"))
                {
                    ReleaseFromHands(ragdollController, heldKitchenObj);
                    PlaceOnCounter(player, heldKitchenObj);

                    bool delivered = deliveryManager.TryMatchAndDeliver(heldKitchenObj);


                    if (delivered)
                    {
                        ClearStationObject(); 
                    }
                    else{
                        Debug.Log("AssembledItem does not match any orders");
                    }
                }
                else 
                {
                    Debug.Log("Item not an assembled dish :(");
                }
            }
            else
            {
                Debug.Log("DeliveryStation already has an object placed on it.");
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

    public override void RemovePlacedKitchenObj(GameObject player)
    {
        if (HasKitchenObject())
        {
            GameObject kitchenObj = GetKitchenObject();
            Rigidbody kitchenObjRb = kitchenObj.GetComponent<Rigidbody>();
            kitchenObjRb.isKinematic = false;

            Vector3 playerPos = player.GetComponent<RagdollController>().centerOfMass.position;

            Vector3 popDirection = (playerPos - transform.position).normalized;
            popDirection.y = 0f;

            kitchenObjRb.AddForce(Vector3.up * verticalForceMultiplier, ForceMode.Impulse);
            kitchenObjRb.AddForce(popDirection * horizontalForceMultiplier, ForceMode.Impulse);

            ClearStationObject();
        }
        else
        {
            Debug.Log("DeliveryStation has no object to remove");
        }
    }

}