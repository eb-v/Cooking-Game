using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// script to handle players reconnecting joints of other players
public class JointReconnection : MonoBehaviour
{
    [SerializeField] private RagdollController ragdollController;
    [SerializeField] private GameObject pelvis;
    private Dictionary<GameObject, JointBackup> storedJointData;
    
    private void Awake()
    {
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(HandleJointReconnection);
        GenericEvent<JointRemoved>.GetEvent(gameObject.name).AddListener(StoreJointData);
        storedJointData = new Dictionary<GameObject, JointBackup>();
    }

    private void HandleJointReconnection(GameObject otherPlayer)
    {
        // is player missing any joints?
        if (PlayerIsMissingParts())
        {
            // get the objects the other player is holding
            GrabDetection leftHandGD = otherPlayer.GetComponent<RagdollController>().leftHand.GetComponent<GrabDetection>();
            GrabDetection rightHandGD = otherPlayer.GetComponent<RagdollController>().rightHand.GetComponent<GrabDetection>();

            
            if (leftHandGD.grabbedObj != null)
            {
                // check if left hand is holding one of the missing joints
                GameObject rootGrabbedObj = leftHandGD.grabbedObj.transform.root.gameObject;
                if (CheckForJointObjMatch(rootGrabbedObj))
                {
                    // if other player is holding a missing joint, disconnect it from their hand
                    GenericEvent<ReleaseHeldJoint>.GetEvent(otherPlayer.name).Invoke(leftHandGD.gameObject);

                    // reconnect the joint to this player
                    ConnectJoint(rootGrabbedObj, storedJointData[rootGrabbedObj], otherPlayer);
                }
                else
                {
                    Debug.Log("Left hand is not holding a missing joint.");
                }
            }

            if (rightHandGD.grabbedObj != null)
            {
                // check if right hand is holding one of the missing joints
                GameObject rootGrabbedObj = rightHandGD.grabbedObj.transform.root.gameObject;
                if (CheckForJointObjMatch(rootGrabbedObj))
                {
                    // if other player is holding a missing joint, disconnect it from their hand
                    GenericEvent<ReleaseHeldJoint>.GetEvent(otherPlayer.name).Invoke(rightHandGD.gameObject);

                    // reconnect the joint to this player
                    ConnectJoint(rootGrabbedObj, storedJointData[rootGrabbedObj], otherPlayer);
                }
                else
                {
                    Debug.Log("Right hand is not holding a missing joint.");
                }
            }
        }
        else
        {
            Debug.Log("Player has all parts, no need to reconnect joints.");
        }
    }

    private bool CheckForJointObjMatch(GameObject grabbedObj)
    {
        foreach (KeyValuePair<GameObject, JointBackup> entry in storedJointData)
        {
            if (grabbedObj == entry.Key)
            {
                return true;
            }
        }
        return false;
    }

    private void StoreJointData(GameObject jointObj, JointBackup jointBackup)
    {
        if (!storedJointData.ContainsKey(jointObj))
        {
            storedJointData.Add(jointObj, jointBackup);
        }
    }

    private void RemoveJointData(GameObject jointObj)
    {
        if (storedJointData.ContainsKey(jointObj))
        {
            storedJointData.Remove(jointObj);
        }
    }

    private bool PlayerIsMissingParts()
    {
        return storedJointData.Count > 0;
    }

    private void ConnectJoint(GameObject jointObjToAttach, JointBackup jointBackup, GameObject playerWhoReconnected)
    {
        SetTagRecursively(jointObjToAttach, "Player");
        RagdollController rc = gameObject.GetComponent<RagdollController>();

        jointObjToAttach.transform.parent = jointBackup.parent;
        
        foreach (RagdollJoint rj in jointObjToAttach.GetComponentsInChildren<RagdollJoint>())
        {
            rc.SetJointToOriginalLocalPosRot(rj);
        }

        ConfigurableJoint joint = jointObjToAttach.GetComponent<ConfigurableJoint>();
        joint.connectedBody = jointBackup.connectedBody;
        joint.connectedAnchor = jointBackup.connectedAnchor;
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        RagdollJoint ragdollJoint = jointObjToAttach.GetComponent<RagdollJoint>();

        Vector3 pelvisPos = pelvis.transform.position;
        pelvisPos.y += 1.3f;
        pelvis.transform.position = pelvisPos;

        foreach (RagdollJoint rj in jointObjToAttach.GetComponentsInChildren<RagdollJoint>())
        {
            rj.isConnected = true;
            rc.ResetReconnectedLimbDrives(rj.GetJointName());
        }

        // if reconnecting a leg, reset the step values to avoid weirdness
        if (ragdollJoint.GetJointName() == "UpperRightLeg" || ragdollJoint.GetJointName() == "UpperLeftLeg")
        {
            rc.ResetStepValues();
        }
        rc.HardResetPose();
        RemoveJointData(jointObjToAttach);

        // Track the joint reconnection stat for the player who helped
        PlayerStats helperStats = playerWhoReconnected.GetComponent<PlayerStats>();
        if (helperStats != null)
        {
            helperStats.IncrementJointsReconnected();
            Debug.Log($"[JOINT RECONNECTED] Player {helperStats.playerNumber} reconnected a joint! Total reconnections: {helperStats.jointsReconnected}");
        }
        else
        {
            Debug.LogWarning($"[JOINT RECONNECTION] {playerWhoReconnected.name} has no PlayerStats component!");
        }
    }

    private void SetTagRecursively(GameObject obj, string newTag)
    {
        obj.tag = newTag;
        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, newTag);
        }
    }
}