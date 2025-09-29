using System.Collections.Generic;
using UnityEngine;

public class Env_Interaction : MonoBehaviour
{
    [SerializeField] private Transform playerCenterofMass;
    [SerializeField] private float interactionRange = 5f;
    private int layerMask;
    private GameObject lastLookedAtKitchenObject;
    public GameObject lookedAtKitchenProp;
    [SerializeField] private float glowIntensity = 0.5f;
    public bool canInteract;
    private GameObject heldObject;
    private Dictionary<string, FixedJoint> grabJoints;

    private void Start()
    {
        layerMask = LayerMask.GetMask("InteractDetectionCollider");

        GenericEvent<ObjectGrabbed>.GetEvent(gameObject.name).AddListener(AssignHeldObj);
        GenericEvent<ObjectReleased>.GetEvent(gameObject.name).AddListener(UnAssignHeldObj);

        GenericEvent<Interact>.GetEvent(gameObject.name).AddListener(PlaceObject);
        GenericEvent<RemovePlacedObject>.GetEvent(gameObject.name).AddListener(RemovePlacedObject);

        GenericEvent<SetUser>.GetEvent(gameObject.name).AddListener(SetCurrentUserForKitchenProp);


        grabJoints = gameObject.GetComponent<RagdollController>().grabJoints;
    }

    private void FixedUpdate()
    {

        RayCastDetection();


    }

    private void ResetHighlight(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = rend.material;
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    private void RayCastDetection()
    {
        // perform raycast in front of player to detect interactable objects
        Ray ray = new Ray(playerCenterofMass.position, playerCenterofMass.forward);

        // if ray cast hits
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, layerMask))
        {
            // assign new hit object to variable
            lookedAtKitchenProp = hit.collider.transform.parent.gameObject;

            canInteract = true;



            if (lastLookedAtKitchenObject != lookedAtKitchenProp)
            {
                if (lastLookedAtKitchenObject != null)
                {
                    ResetHighlight(lastLookedAtKitchenObject);
                }
            }

            lastLookedAtKitchenObject = lookedAtKitchenProp;

            // highlight the currently hit object
            Renderer rend = lookedAtKitchenProp.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = rend.material;
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.white * glowIntensity);
            }
            Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.green);


        }
        else
        {
            if (lastLookedAtKitchenObject != null && lastLookedAtKitchenObject.GetComponent<IPrepStation>().currentUser == gameObject)
            {
                GenericEvent<PlayerStoppedLookingAtKitchenStation>.GetEvent(lastLookedAtKitchenObject.name).Invoke();
            }




            lookedAtKitchenProp = null;
            if (lastLookedAtKitchenObject != null)
            {
                ResetHighlight(lastLookedAtKitchenObject);
                lastLookedAtKitchenObject = null;
            }
            canInteract = false;
            Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);
        }
    }

    private void PlaceObject()
    {
        if (lookedAtKitchenProp == null) return;
        
        



        if (lookedAtKitchenProp.GetComponent<IPrepStation>().isBeingUsed) return;

        if (lookedAtKitchenProp.GetComponent<IPrepStation>().containsObject)
        {
            Debug.Log("Counter already has an object placed on it.");
            return;
        }

        if (canInteract && heldObject != null && lookedAtKitchenProp != null)
        {
            if (heldObject.tag != "Ingredient") return;

            //destroy grab joint that connects the hand to the object
            foreach (KeyValuePair<string, FixedJoint> entry in grabJoints)
            {
                GameObject grabbedObject = entry.Value.gameObject.GetComponent<FixedJoint>().connectedBody.gameObject;

                if (grabbedObject == heldObject)
                {
                    entry.Value.gameObject.GetComponentInChildren<GrabDetection>().isGrabbing = false;

                    Destroy(entry.Value);
                    grabJoints.Remove(entry.Key);
                    break;
                }

            }


            // place object onto the snap point of the hit object
            Collider counterCollider = lookedAtKitchenProp.GetComponent<Collider>();

            float counterYOffset = counterCollider.bounds.extents.y;

            Vector3 placePos = counterCollider.bounds.center;
            placePos.y += counterYOffset;

            Collider heldObjColider = heldObject.GetComponent<Collider>();

            //float heldObjYOffset = heldObjColider.bounds.extents.y;
            //placePos.y += heldObjYOffset;

            heldObject.transform.position = placePos;
            heldObject.transform.rotation = Quaternion.identity;
            heldObject.GetComponent<Rigidbody>().isKinematic = true;

            IPrepStation kitchenStationObj = lookedAtKitchenProp.GetComponent<IPrepStation>();
            kitchenStationObj.currentPlacedObject = heldObject;
            kitchenStationObj.containsObject = true;

            heldObject = null;

        }

    }

    // remove the placed object from the counter and apply small force to make it pop off
    private void RemovePlacedObject()
    {

        if (canInteract && lookedAtKitchenProp != null && heldObject == null)
        {
             IPrepStation kitchenStationObj = lookedAtKitchenProp.GetComponent<IPrepStation>();
            if (!kitchenStationObj.containsObject) return;

            GameObject placedObj = kitchenStationObj.currentPlacedObject;
            

            // pop object off counter
            placedObj.GetComponent<Rigidbody>().isKinematic = false;
            placedObj.GetComponent<Rigidbody>().AddForce(Vector3.up * 6f, ForceMode.Impulse);
            Vector3 popDirection = (playerCenterofMass.position - lookedAtKitchenProp.transform.position).normalized;
            popDirection.y = 0f;

            placedObj.GetComponent<Rigidbody>().AddForce(popDirection * 6f, ForceMode.Impulse);

            kitchenStationObj.containsObject = false;
            kitchenStationObj.currentPlacedObject = null;
            GenericEvent<ObjectRemovedFromKitchenStation>.GetEvent(lookedAtKitchenProp.name).Invoke();

            
        }
    }



    private void AssignHeldObj(GameObject grabbedObj)
    {
        if (heldObject != null) return;

        heldObject = grabbedObj;

    }

    private void UnAssignHeldObj()
    {
        heldObject = null;
    }

    private void SetCurrentUserForKitchenProp()
    {
        GameObject user = lookedAtKitchenProp.GetComponent<IPrepStation>().currentUser;

        // is player looking at a kitchen prop that has an object and the kitchen prop is not being used by another player?
        if (lookedAtKitchenProp != null && user == null && lookedAtKitchenProp.GetComponent<IPrepStation>().containsObject)
        {
            lookedAtKitchenProp.GetComponent<IPrepStation>().currentUser = gameObject;
        }
    }
}
