using System.Collections.Generic;
using UnityEngine;

public class SavePose : MonoBehaviour
{
    private RagdollController rc;

    [SerializeField] private UDictionary<string, Vector3> jointTargetRotations;
    [Header("Right Arm Rotations")]
    [SerializeField] private Vector3 upperRightArmRotation;
    [SerializeField] private Vector3 lowerRightArmRotation;
    [Header("Left Arm Rotations")]
    [SerializeField] private Vector3 upperLeftArmRotation;
    [SerializeField] private Vector3 lowerLeftArmRotation;
    [Header("Head and Body Rotations")]
    [SerializeField] private Vector3 headRotation;
    [SerializeField] private Vector3 bodyRotation;
    [Header("Pose Data")]
    [SerializeField] private PoseData poseData;


    private void Awake()
    {
        if (TryGetComponent<RagdollController>(out RagdollController ragdollController))
        {
            rc = ragdollController;
        }
        else
        {
            Debug.LogError("RagdollController component not found on the GameObject.");
        }

        foreach (KeyValuePair<string, RagdollJoint> entry in rc.RagdollDict)
        {
            if (entry.Key == "Root" || entry.Key == "UpperRightLeg"
                || entry.Key == "UpperLeftLeg" || entry.Key == "LowerLeftLeg"
                || entry.Key == "UpperRightLeg" || entry.Key == "LowerRightLeg"
                || entry.Key == "RightFoot" || entry.Key == "LeftFoot")
                continue;


            RagdollJoint joint = entry.Value;
            jointTargetRotations[entry.Key] = joint.TargetRotationEuler;
        }

    }

    private void Update()
    {
        jointTargetRotations["UpperRightArm"] = upperRightArmRotation;
        jointTargetRotations["LowerRightArm"] = lowerRightArmRotation;
        jointTargetRotations["UpperLeftArm"] = upperLeftArmRotation;
        jointTargetRotations["LowerLeftArm"] = lowerLeftArmRotation;
        jointTargetRotations["Head"] = headRotation;
        jointTargetRotations["Body"] = bodyRotation;


        foreach (KeyValuePair<string, Vector3> entry in jointTargetRotations)
        {
            rc.TargetRotations[entry.Key] = Quaternion.Euler(entry.Value);
        }
    }


    public void SavePoseFunction()
    {
        foreach (KeyValuePair<string, RagdollJoint> entry in rc.RagdollDict)
        {
            poseData.poseJointTargetRotations[entry.Key] = entry.Value.Joint.targetRotation;
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(poseData);
#endif
        Debug.Log("Pose Data Saved");
    }
}
