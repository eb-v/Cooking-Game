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
        RagdollJoint ragdollJoint = jointObjToAttach.GetComponent<RagdollJoint>();
        string jointName = jointObjToAttach.GetComponent<RagdollJoint>().GetJointName();
        Rigidbody jointRb = jointObjToAttach.GetComponent<Rigidbody>();

        ConfigurableJoint joint = jointObjToAttach.AddComponent<ConfigurableJoint>();
        ragdollJoint.SetConfigurableJoint(joint);

        JointSettingsData backupData = rc.GetJointBackUpData(jointName);

        Quaternion desiredWorldRotation = backupData.parent.rotation * backupData.localRotation;

        // 2. Apply the DESIRED WORLD ROTATION
        // Apply World Rotation directly to the Transform.
        // This forces the limb into the correct final orientation regardless of parent's current rotation.
        jointObjToAttach.transform.rotation = desiredWorldRotation;



        rc.RestoreJointData(joint, jointName);



        jointRb.isKinematic = true;


        // Alligns the joint anchor to the correct position it should be at
        AdjustJointAnchor(joint, backupData, jointObjToAttach);









        // move pelvis up to avoid clipping into ground when reconnecting joints
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

        jointRb.isKinematic = false;
        // update the dictionaries in RagdollController
        rc.UpdateJointDictionaries(jointObjToAttach);
        // set target rotations to original rotations
        rc.HardResetPose();




        // set the local pos/rot to original
        //foreach (RagdollJoint rj in jointObjToAttach.GetComponentsInChildren<RagdollJoint>())
        //{
        //    rc.SetJointToOriginalLocalPosRot(rj);
        //}


        // play joint reconnect SFX
        //AudioManager.Instance?.PlaySFX("Joint reconnect");
        // Track the joint reconnection stat for the player who helped
        //PlayerStats helperStats = playerWhoReconnected.GetComponent<PlayerStats>();
        //if (helperStats != null)
        //{
        //    helperStats.IncrementJointsReconnected();
        //    Debug.Log($"[JOINT RECONNECTED] Player {helperStats.playerNumber} reconnected a joint! Total reconnections: {helperStats.jointsReconnected}");
        //}
        //else
        //{
        //    Debug.LogWarning($"[JOINT RECONNECTION] {playerWhoReconnected.name} has no PlayerStats component!");
        //}
    }

    private void AdjustJointAnchor(ConfigurableJoint joint, JointSettingsData backupData, GameObject jointObjToAttach)
    {
        Vector3 targetWorldPosition = backupData.connectedBody.transform.TransformPoint(backupData.connectedAnchor);

        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = backupData.connectedAnchor; // This must be set before anchor to avoid issues
        joint.anchor = backupData.anchor;

        Vector3 anchorWorldPosition = jointObjToAttach.transform.TransformPoint(joint.anchor);
        Vector3 correctionVector = targetWorldPosition - anchorWorldPosition;


        jointObjToAttach.transform.position += correctionVector;
        jointObjToAttach.transform.localRotation = backupData.localRotation;

        Rigidbody jointRb = jointObjToAttach.GetComponent<Rigidbody>();

        jointRb.transform.position = jointObjToAttach.transform.position;
        jointRb.transform.rotation = jointObjToAttach.transform.rotation;

        jointRb.linearVelocity = Vector3.zero;
        jointRb.angularVelocity = Vector3.zero;
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