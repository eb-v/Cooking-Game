using System.Collections.Generic;
using UnityEngine;

public class Env_Interaction : MonoBehaviour
{
    [SerializeField] private Transform playerCenterofMass;
    [SerializeField] private float interactionRange = 5f;
    private int layerMask;
    private GameObject lastHit, hitObject;
    [SerializeField] private float glowIntensity = 0.5f;
    public bool canInteract;
    private GameObject heldObject;
    private Dictionary<string, FixedJoint> grabJoints;

    private void Start()
    {
        layerMask = LayerMask.GetMask("Interactable");

        GenericEvent<ObjectGrabbed>.GetEvent(gameObject.name).AddListener(AssignHeldObj);
        GenericEvent<ObjectReleased>.GetEvent(gameObject.name).AddListener(UnAssignHeldObj);

        GenericEvent<Interact>.GetEvent(gameObject.name).AddListener(PlaceObject);

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

        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);

        // if ray cast hits
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, layerMask))
        {
            // assign new hit object to variable
            hitObject = hit.collider.gameObject;

            canInteract = true;

            if (lastHit != hitObject)
            {
                if (lastHit != null)
                {
                    ResetHighlight(lastHit);
                }
            }

            lastHit = hitObject;

            // highlight the currently hit object
            Renderer rend = hitObject.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = rend.material;
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.white * glowIntensity);
            }

        }
        else
        {
            hitObject = null;
            if (lastHit != null)
            {
                ResetHighlight(lastHit);
                lastHit = null;
            }
            canInteract = false;
        }
    }

    private void PlaceObject()
    {
        
        if (canInteract && heldObject != null && hitObject != null)
        {
            if (heldObject.tag != "Interactable") return;

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
            Renderer rend = heldObject.GetComponentInChildren<Renderer>();

            float itemBottom = rend.bounds.min.y;

            float counterTop = hitObject.GetComponent<Renderer>().bounds.max.y;

            //Vector3 itemPos = hitObject.transform.Find("SnapPoint").position;

            Vector3 itemPos = hitObject.transform.position;

            float offset = counterTop - itemBottom;

            itemPos.y += offset;

            heldObject.transform.position = itemPos;

            heldObject.transform.rotation = Quaternion.identity;

            //heldObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            heldObject.GetComponent<Rigidbody>().isKinematic = true;

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
}
