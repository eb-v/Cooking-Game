using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Grab System", menuName = "Systems/Grab System")]
public class GrabSystem : ScriptableObject
{
    private static GrabSystem instance;
    public static GrabSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GrabSystem>("Grab System");
            }
            return instance;
        }
    }

    [SerializeField] private bool systemEnabled = true;
    public static bool SystemEnabled
    {
        get { return Instance.systemEnabled; }
        set { Instance.systemEnabled = value; }
    }

    private void OnEnable()
    {
        SystemEnabled = systemEnabled;
    }

    private void OnValidate()
    {
        SystemEnabled = systemEnabled;
    }

    // attach object to hand
    public static void GrabObject(GameObject hand, GameObject objToGrab)
    {
        if (!SystemEnabled)
            return;

        if (hand.GetComponent<GrabScript>().isGrabbing == true)
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

        hand.GetComponent<GrabScript>().isGrabbing = true;
        hand.GetComponent<GrabScript>().grabbedObj = objToGrab;

        // ADD THIS: Track ingredient handling
        //if (objToGrab.CompareTag("Ingredient") || objToGrab.GetComponent<Ingredient>() != null)
        //{
        //    ////PlayerStats stats = GetComponent<PlayerStats>();
        //    //if (stats != null)
        //    //{
        //    //    stats.IncrementIngredientsHandled();
        //    //}
        //}
    }
    // detach object from hand
    public static void ReleaseObject(GameObject hand)
    {
        if (!SystemEnabled)
            return;

        if (hand.GetComponent<GrabScript>().isGrabbing == false)
            return;

        if (hand.transform.parent.gameObject.GetComponent<FixedJoint>() != null)
        {
            Destroy(hand.transform.parent.gameObject.GetComponent<FixedJoint>());
        }
        hand.GetComponent<GrabScript>().isGrabbing = false;

        hand.GetComponent<GrabScript>().grabbedObj = null;
    }

}
