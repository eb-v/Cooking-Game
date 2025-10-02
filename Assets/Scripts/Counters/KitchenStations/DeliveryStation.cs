using UnityEngine;

public class DeliveryStation : BaseStation {
    private void Awake()
    {
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(Interact);
    }

    //copied over from CuttingStation.cs with some changes
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
                // if held object assembleditem
                if (heldKitchenObj.CompareTag("AssembledItem"))
                {
                    // logic for detaching object from hand

                    if (ragdollController.leftHand.GetComponent<GrabDetection>().isGrabbing)
                    {
                        GameObject grabbedObj = ragdollController.leftHand.GetComponent<GrabDetection>().grabbedObj;

                        if (grabbedObj == heldKitchenObj)
                        {
                            GameObject leftArm = ragdollController.leftHand.transform.parent.gameObject;
                            Destroy(leftArm.GetComponent<FixedJoint>());
                            ragdollController.leftHand.GetComponent<GrabDetection>().isGrabbing = false;
                            ragdollController.leftHand.GetComponent<GrabDetection>().grabbedObj = null;
                        }
                    }

                    if (ragdollController.rightHand.GetComponent<GrabDetection>().isGrabbing)
                    {
                        GameObject grabbedObj = ragdollController.rightHand.GetComponent<GrabDetection>().grabbedObj;
                        if (grabbedObj == heldKitchenObj)
                        {
                            GameObject rightArm = ragdollController.rightHand.transform.parent.gameObject;
                            Destroy(rightArm.GetComponent<FixedJoint>());
                            ragdollController.rightHand.GetComponent<GrabDetection>().isGrabbing = false;
                            ragdollController.rightHand.GetComponent<GrabDetection>().grabbedObj = null;
                        }
                    }

                    // logic for placing object on counter
                    Collider stationCollider = player.GetComponent<Env_Interaction>().currentlyLookingAt.GetComponent<Collider>();

                    float stationYOffset = stationCollider.bounds.extents.y;
                    Vector3 placePos = stationCollider.bounds.center;
                    placePos.y += stationYOffset;

                    Collider heldObjCollider = heldKitchenObj.GetComponent<Collider>();
                    heldKitchenObj.transform.position = placePos;
                    heldKitchenObj.transform.rotation = Quaternion.identity;
                    heldKitchenObj.GetComponent<Rigidbody>().isKinematic = true;
                    SetStationObject(heldKitchenObj);

                    //destroy after 1 
                    BaseKitchenObject bko = heldKitchenObj.GetComponent<BaseKitchenObject>();
                    Destroy(bko.gameObject, 1f);
                    ClearStationObject(); 
                    //GenericEvent<DeliveredDishEvent>.GetEvent(gameObject.name).Invoke();

                    if (PointManager.Instance != null)
                        PointManager.Instance.AddDeliveredDish();
                    else
                        Debug.LogError("PointManager.Instance is null!");

                    Debug.Log("Assembled Dish Delivered!");
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

    //remove logic
    public override void RemovePlacedKitchenObj(GameObject player)
    {
        base.RemovePlacedKitchenObj(player);
    }

}