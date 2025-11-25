using UnityEngine;
using System.Collections.Generic;

public class PoseHelper
{
    private static void ZeroJointVelocity(ConfigurableJoint joint)
    {
        Rigidbody rb = joint.GetComponent<Rigidbody>();
        Rigidbody connected = joint.connectedBody;

        // Zero body holding the joint
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Zero connected body (if any)
        if (connected != null)
        {
            connected.linearVelocity = Vector3.zero;
            connected.angularVelocity = Vector3.zero;
        }
    }


    private static void SnapJointToTarget(ConfigurableJoint joint)
    {
        Quaternion target = joint.targetRotation;

        Quaternion initialLocal = joint.transform.localRotation;
        if (joint.connectedBody == null)
            return;

        Quaternion connectedWorld = joint.connectedBody.rotation;


        Quaternion worldRot =
                connectedWorld *
                target *
                Quaternion.Inverse(initialLocal);

        joint.transform.rotation = worldRot;
    }

    public static void SafeSnapToTarget(ConfigurableJoint joint)
    {
        Rigidbody rb = joint.GetComponent<Rigidbody>();
        Rigidbody connected = joint.connectedBody;

        //// Save state
        //bool oldKinematic = rb.isKinematic;
        //bool oldConnectedKin = connected != null ? connected.isKinematic : false;

        //// Disable physics briefly
        //rb.isKinematic = true;
        //if (connected != null)
        //    connected.isKinematic = true;

        // Snap the rotation
        SnapJointToTarget(joint);  // <-- your snapping function here

        // Zero velocities (just in case)
        ZeroJointVelocity(joint);

        //// Restore physics state
        //rb.isKinematic = oldKinematic;
        //if (connected != null)
        //    connected.isKinematic = oldConnectedKin;
    }

    public static void SetPlayerPoseAndSnap(RagdollController rc, PoseData poseData)
    {
        foreach (KeyValuePair<string, Quaternion> kvp in poseData.poseJointTargetRotations)
        {
            rc.TargetRotations[kvp.Key] = kvp.Value;
            ConfigurableJoint joint = rc.RagdollDict[kvp.Key].Joint;
            PoseHelper.SafeSnapToTarget(joint);
        }
    }

    public static void SetPlayerPose(RagdollController rc, PoseData poseData)
    {
        foreach (KeyValuePair<string, Quaternion> kvp in poseData.poseJointTargetRotations)
        {
            rc.TargetRotations[kvp.Key] = kvp.Value;
        }
    }
}
