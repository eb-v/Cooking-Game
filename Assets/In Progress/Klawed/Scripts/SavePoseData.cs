using System.Collections.Generic;
using UnityEngine;

public class SavePoseData : MonoBehaviour
{
    [SerializeField] private RagdollController rc;
    [SerializeField] private PoseData poseData;

    [Header("Right Arm")]
    [SerializeField] private Vector3 UpperRightArm;
    [SerializeField] private Vector3 LowerRightArm;

    [Header("Left Arm")]
    [SerializeField] private Vector3 UpperLeftArm;
    [SerializeField] private Vector3 LowerLeftArm;


    private void Start()
    {
        rc.GetPelvis().GetComponent<Rigidbody>().isKinematic = true;

        GameObject player = rc.gameObject;

        foreach (Collider collider in player.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        // disable all configurable joints


        foreach (KeyValuePair<string, RagdollJoint> kvp in rc.RagdollDict)
        {
            if (poseData.poseJointTargetRotations.ContainsKey(kvp.Key))
            {
                kvp.Value.Joint.targetRotation = poseData.poseJointTargetRotations[kvp.Key];
            }
        }

    }



    public void SavePoseDataFunction()
    {
        foreach (KeyValuePair<string, RagdollJoint> kvp in rc.RagdollDict)
        {
            poseData.poseJointTargetRotations[kvp.Key] = kvp.Value.Joint.targetRotation;
        }

        

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(poseData);
#endif
        Debug.Log("Pose Data Saved");
    }

    public void SetToTargetJointRotation()
    {
        foreach (KeyValuePair<string, RagdollJoint> kvp in rc.RagdollDict)
        {
            if (poseData.poseJointTargetRotations.ContainsKey(kvp.Key))
            {
                kvp.Value.Joint.targetRotation = poseData.poseJointTargetRotations[kvp.Key];
            }
        }
    }

   


    private void Update()
    {
       // rc.UpdateLogic();
        rc.SetArmRotationValues(UpperLeftArm, LowerLeftArm, UpperRightArm, LowerRightArm);

        

        
    }

    private void FixedUpdate()
    {
        //rc.FixedUpdateLogic();
    }

}
