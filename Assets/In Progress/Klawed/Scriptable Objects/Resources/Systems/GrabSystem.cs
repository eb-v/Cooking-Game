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
                instance = Resources.Load<GrabSystem>("Systems/Grab System");
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
    
    private static int GetPlayerNumber(GameObject hand)
    {
        // Check the hand's hierarchy for PlayerStats component
        PlayerStats playerStats = hand.GetComponentInParent<PlayerStats>();
        if (playerStats != null)
        {
            return playerStats.playerNumber;
        }
        
        // Fallback: Try to find the player in PlayerManager's list by checking hierarchy
        if (PlayerManager.Instance != null)
        {
            Transform current = hand.transform;
            
            // Go up the hierarchy to find which player this hand belongs to
            while (current != null)
            {
                for (int i = 0; i < PlayerManager.Instance.players.Count; i++)
                {
                    if (PlayerManager.Instance.players[i] == current.gameObject)
                    {
                        return i + 1; // Player numbers are 1-indexed
                    }
                }
                current = current.parent;
            }
        }
        
        // Default to 1 if not found
        Debug.LogWarning($"Could not find player number for hand: {hand.name}. Defaulting to Player 1.");
        return 1;
    }
    
    // attach object to hand
    public static void GrabObject(GameObject hand, GameObject objToGrab)
    {
        if (!SystemEnabled)
            return;
        
        GrabScript grabScript = hand.GetComponent<GrabScript>();
        
        if (grabScript == null || grabScript.isGrabbing)
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
        
        grabScript.isGrabbing = true;
        grabScript.grabbedObj = objToGrab;
        
        // Track items grabbed
        if (PlayerStatsManager.Instance != null)
        {
            int playerNumber = GetPlayerNumber(hand);
            PlayerStatsManager.Instance.IncrementItemsGrabbed(playerNumber);
        }
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