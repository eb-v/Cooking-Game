using System.Runtime.CompilerServices;
using UnityEngine;

public class GrabSystem : MonoBehaviour
{


    private void OnEnable()
    {
        GenericEvent<OnHandCollisionEnter>.GetEvent("GrabSystem").AddListener(GrabObject);
        GenericEvent<ReleaseHeldJoint>.GetEvent("GrabSystem").AddListener(ReleaseObject);
        GenericEvent<PlacedIngredient>.GetEvent("GrabSystem").AddListener(ReleaseObject);
        GenericEvent<RightGrabInputCanceled>.GetEvent("GrabSystem").AddListener(ReleaseObject);
        GenericEvent<LeftGrabInputCanceled>.GetEvent("GrabSystem").AddListener(ReleaseObject);  

    }

    private void OnDisable()
    {
        GenericEvent<OnHandCollisionEnter>.GetEvent("GrabSystem").RemoveListener(GrabObject);
        GenericEvent<ReleaseHeldJoint>.GetEvent("GrabSystem").RemoveListener(ReleaseObject);
    }




    // attach object to hand
    private void GrabObject(GameObject hand, GameObject objToGrab)
    {
        if (hand.GetComponent<GrabDetection>().isGrabbing == true)
            return;

        FixedJoint grabJoint = hand.transform.parent.gameObject.AddComponent<FixedJoint>();

        if (objToGrab.GetComponent<Rigidbody>() == null)
        {
            grabJoint.connectedBody = objToGrab.transform.parent.GetComponent<Rigidbody>();
        }
        else
        {
            grabJoint.connectedBody = objToGrab.GetComponent<Rigidbody>();
        }

        hand.GetComponent<GrabDetection>().isGrabbing = true;
        hand.GetComponent<GrabDetection>().grabbedObj = objToGrab;

        // ADD THIS: Track ingredient handling
        if (objToGrab.CompareTag("Ingredient") || objToGrab.GetComponent<Ingredient>() != null)
        {
            PlayerStats stats = GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.IncrementIngredientsHandled();
            }
        }
    }
    // detach object from hand
    private void ReleaseObject(GameObject hand)
    {
        if (hand.GetComponent<GrabDetection>().isGrabbing == false)
            return;

        if (hand.transform.parent.gameObject.GetComponent<FixedJoint>() != null)
        {
            Destroy(hand.transform.parent.gameObject.GetComponent<FixedJoint>());
        }
        hand.GetComponent<GrabDetection>().isGrabbing = false;

        hand.GetComponent<GrabDetection>().grabbedObj = null;
    }

}
