using UnityEngine;

public class CuttingStation : BaseStation
{

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private void Awake()
    {
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(Interact);
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
                // if that something is an ingredient that can be cut
                if (HasRecipeWithInput(heldKitchenObj))
                {
                    // logic for detaching object from hand


                    // find out which hand is holding the object

                    // if left hand is holding something
                    if (ragdollController.leftHand.GetComponent<GrabDetection>().isGrabbing)
                    {
                        GameObject grabbedObj = ragdollController.leftHand.GetComponent<GrabDetection>().grabbedObj;
                        // is the object held by the left hand the same as the one we are checking?
                        if (grabbedObj == heldKitchenObj)
                        {
                            // remove fixed joint from left hand and place object on counter
                            GameObject leftArm = ragdollController.leftHand.transform.parent.gameObject;
                            Destroy(leftArm.GetComponent<FixedJoint>());
                            ragdollController.leftHand.GetComponent<GrabDetection>().isGrabbing = false;
                            ragdollController.leftHand.GetComponent<GrabDetection>().grabbedObj = null;
                        }
                    }

                    if (ragdollController.rightHand.GetComponent<GrabDetection>().isGrabbing)
                    {
                        GameObject grabbedObj = ragdollController.rightHand.GetComponent<GrabDetection>().grabbedObj;
                        // is the object held by the right hand the same as the one we are checking?
                        if (grabbedObj == heldKitchenObj)
                        {
                            // remove fixed joint from right hand and place object on counter
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
                }
                else
                {
                    Debug.Log("Player not holding an ingredient that can be cut");
                }

            }
            else
            {
                Debug.Log("CuttingStation already has an object placed on it.");
            }
        }
        else
        {
            Debug.Log("Player not carrying anything");
        }

    }

    public override void RemovePlacedKitchenObj()
    {
        base.RemovePlacedKitchenObj();
    }

    private bool HasRecipeWithInput(GameObject kitchenObject)
    {
        CuttingRecipeSO cuttingRecipe = GetCuttingRecipeWithInput(kitchenObject);
        return cuttingRecipe != null;
    }

    private CuttingRecipeSO GetCuttingRecipeWithInput(GameObject kitchenObject)
    {
        string kitchenObjectName = kitchenObject.name.Replace("(Clone)", "").Trim();

        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input.name == kitchenObjectName)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

}
