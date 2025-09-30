using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;


public class PlaceIngredientScript : MonoBehaviour
{
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private Transform playerCenterofMass;
    private GameObject objLookingAt;
    private GameObject objBeingHeld;

    private void Start()
    {
        GenericEvent<InteractableLookedAtChanged>.GetEvent(gameObject.name).AddListener(AssignObjLookingAt);
        GenericEvent<ObjectGrabbed>.GetEvent(gameObject.name).AddListener(AddHeldObj);
        GenericEvent<ObjectReleased>.GetEvent(gameObject.name).AddListener(RemoveHeldObj);

        GenericEvent<OnPlaceIngredientInput>.GetEvent(gameObject.name).AddListener(PlaceIngredient);

        GenericEvent<RemovePlacedObject>.GetEvent(gameObject.name).AddListener(RemovePlacedObj);
    }

    private void AssignObjLookingAt(GameObject obj)
    {
        objLookingAt = obj;
    }

    private void AddHeldObj(GameObject grabbedObj)
    {
        // if obj already in array, do nothing
        objBeingHeld = grabbedObj;
    }

    private void RemoveHeldObj(GameObject grabbedObj)
    {
        objBeingHeld = null;
    }

    private void PlaceIngredient()
    {
        if (objLookingAt == null)
        {
            Debug.Log("No object to place ingredient on");
            return;
        }

        if (objLookingAt.GetComponent<IPrepStation>().containsObject)
        {
            //RemovePlacedObj();
            //PlaceIngredient();
            Debug.Log("Counter already has an object placed on it.");
        }

        if (objBeingHeld == null)
        {
            Debug.Log("No ingredient being held");
            return;
        }


        if (objBeingHeld.tag != "Ingredient")
        {
            Debug.Log("Object being held is not an ingredient");
            return;
        }

        if (leftHand.transform.parent.gameObject.GetComponent<FixedJoint>() != null)
        {
            if (leftHand.transform.parent.gameObject.GetComponent<FixedJoint>().connectedBody.gameObject == objBeingHeld)
            {
                Destroy(leftHand.transform.parent.gameObject.GetComponent<FixedJoint>());
                leftHand.GetComponent<GrabDetection>().isGrabbing = false;
                leftHand.GetComponent<GrabDetection>().grabbedObj = null;
            }
        }

        if (rightHand.transform.parent.gameObject.GetComponent<FixedJoint>() != null)
        {
            if (rightHand.transform.parent.gameObject.GetComponent<FixedJoint>().connectedBody.gameObject == objBeingHeld)
            {
                Destroy(rightHand.transform.parent.gameObject.GetComponent<FixedJoint>());
                rightHand.GetComponent<GrabDetection>().isGrabbing = false;
                rightHand.GetComponent<GrabDetection>().grabbedObj = null;
            }
        }

        Collider counterCollider = objLookingAt.GetComponent<Collider>();

        float counterYOffset = counterCollider.bounds.extents.y;

        Vector3 placePos = counterCollider.bounds.center;
        placePos.y += counterYOffset;

        Collider heldObjCollider = objBeingHeld.GetComponent<Collider>();

        objBeingHeld.transform.position = placePos;
        objBeingHeld.transform.rotation = Quaternion.identity;
        objBeingHeld.GetComponent<Rigidbody>().isKinematic = true;

        IPrepStation kitchenStationObj = objLookingAt.GetComponent<IPrepStation>();
        kitchenStationObj.currentPlacedObject = objBeingHeld;
        kitchenStationObj.containsObject = true;
        objBeingHeld = null;


    }


    private void RemovePlacedObj()
    {
        if (objLookingAt != null)
        {
            if (!objLookingAt.GetComponent<IPrepStation>().containsObject) return;

            IPrepStation kitchenStationObj = objLookingAt.GetComponent<IPrepStation>();
            if (!kitchenStationObj.containsObject) return;

            GameObject placedObj = kitchenStationObj.currentPlacedObject;


            // pop object off counter
            placedObj.GetComponent<Rigidbody>().isKinematic = false;
            placedObj.GetComponent<Rigidbody>().AddForce(Vector3.up * 6f, ForceMode.Impulse);
            Vector3 popDirection = (playerCenterofMass.position - objLookingAt.transform.position).normalized;
            popDirection.y = 0f;

            placedObj.GetComponent<Rigidbody>().AddForce(popDirection * 6f, ForceMode.Impulse);

            kitchenStationObj.containsObject = false;
            kitchenStationObj.currentPlacedObject = null;
            GenericEvent<ObjectRemovedFromKitchenStation>.GetEvent(objLookingAt.name).Invoke();


        }
    }
}
