using System.Collections.Generic;
using UnityEngine;

public class SavePose : MonoBehaviour
{
    private RagdollController rc;

    [SerializeField] private UDictionary<string, Vector3> jointTargetRotations;


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
        foreach (KeyValuePair<string, Vector3> entry in jointTargetRotations)
        {
            string jointName = entry.Key;
            Vector3 eulerRot = entry.Value;

            ConfigurableJoint joint = rc.RagdollDict[jointName].Joint;
            joint.targetRotation = Quaternion.Euler(eulerRot);
        }
    }


    public void SavePoseFunction()
    {
        
    }
}
