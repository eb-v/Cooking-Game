using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Interactable))]
// script to handle players reconnecting joints of other players
public class JointReconnection : MonoBehaviour
{
    [SerializeField] private RagdollController ragdollController;
    [SerializeField] private GameObject pelvis;
    //private Dictionary<GameObject, JointBackup> storedJointData;

    private void Awake()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnInteract);
        GenericEvent<JointRemoved>.GetEvent(gameObject.name).AddListener(StoreJointData);
    }

    private void OnInteract(GameObject player)
    {
        Debug.Log("Attempting to reconnect joint...");
        TryToReconnectJoint(player);
    }

    private bool TryToReconnectJoint(GameObject otherPlayer)
    {
        GrabScript grabScript = otherPlayer.GetComponent<GrabScript>();
        if (grabScript.IsGrabbing)
        {
            Grabable grabable = grabScript.grabbedObject;
            GameObject grabbedObj = grabable.gameObject;
            if (grabbedObj.TryGetComponent<RagdollJoint>(out RagdollJoint ragdollJoint))
            {
                if (CheckForJointObjMatch(ragdollJoint))
                {
                    
                    grabable.Release();
                    ConnectJoint(grabbedObj, otherPlayer);
                    return true;
                }
                else
                {
                    Debug.Log("CheckforJointObjMatch failed");
                }
            }
        }

        return false;
    }

    private bool CheckForJointObjMatch(RagdollJoint ragdollJoint)
    {
        //foreach (KeyValuePair<GameObject, JointBackup> entry in storedJointData)
        //{
        //    if (grabbedObj == entry.Key)
        //    {
        //        return true;
        //    }
        //}
        //return false;
        RagdollController rc = gameObject.GetComponent<RagdollController>();
        return rc.disconnectedJoints.Contains(ragdollJoint.gameObject);
    }

    private void StoreJointData(GameObject jointObj, JointBackup jointBackup)
    {
        //if (!storedJointData.ContainsKey(jointObj))
        //{
        //    storedJointData.Add(jointObj, jointBackup);
        //}
    }

    private void RemoveJointData(GameObject jointObj)
    {
        //if (storedJointData.ContainsKey(jointObj))
        //{
        //    storedJointData.Remove(jointObj);
        //}
    }

    private bool PlayerIsMissingParts()
    {
        //return storedJointData.Count > 0;
        return false;
    }

    private void ConnectJoint(GameObject jointObjToAttach, GameObject playerWhoReconnected)
    {
        RagdollController rc = gameObject.GetComponent<RagdollController>();
        string jointName = jointObjToAttach.GetComponent<RagdollJoint>().GetJointName();

        ConfigurableJoint joint = jointObjToAttach.AddComponent<ConfigurableJoint>();

        rc.RestoreJointData(joint, jointName);

        foreach (RagdollJoint rj in jointObjToAttach.GetComponentsInChildren<RagdollJoint>())
        {
            rc.SetJointToOriginalLocalPosRot(rj);
        }


        SetTagRecursively(jointObjToAttach, "Player");

        //ConfigurableJoint joint = jointObjToAttach.GetComponent<ConfigurableJoint>();
        //joint.connectedBody = jointBackup.connectedBody;
        //joint.connectedAnchor = jointBackup.connectedAnchor;
        //joint.xMotion = ConfigurableJointMotion.Locked;
        //joint.yMotion = ConfigurableJointMotion.Locked;
        //joint.zMotion = ConfigurableJointMotion.Locked;
        //RagdollJoint ragdollJoint = jointObjToAttach.GetComponent<RagdollJoint>();

        Vector3 pelvisPos = pelvis.transform.position;
        pelvisPos.y += 1.3f;
        pelvis.transform.position = pelvisPos;

        foreach (RagdollJoint rj in jointObjToAttach.GetComponentsInChildren<RagdollJoint>())
        {
            rj.isConnected = true;
            rc.ResetReconnectedLimbDrives(rj.GetJointName());
        }

        // if reconnecting a leg, reset the step values to avoid weirdness
        if (jointName == "UpperRightLeg" || jointName == "UpperLeftLeg")
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

        // play joint reconnect SFX
        AudioManager.Instance?.PlaySFX("Joint reconnect");
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