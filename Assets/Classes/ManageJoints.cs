using UnityEngine;

public static class ManageJoints
{
    public static void SetTargetRotation(ConfigurableJoint joint, Quaternion targetWorldRotation)
    {
        Quaternion jointSpaceRotation;

        if (joint.connectedBody != null)
        {
            jointSpaceRotation = Quaternion.Inverse(joint.connectedBody.rotation) * targetWorldRotation;
        }
        else
        {
            jointSpaceRotation = targetWorldRotation;
        }

        joint.targetRotation = jointSpaceRotation;
    }

    public static bool IsJointAtTargetRotation(ConfigurableJoint joint, Quaternion targetWorldRotation, float toleranceDegrees = 5f)
    {
        Quaternion current = joint.transform.rotation;

        float angleDifference = Quaternion.Angle(current, targetWorldRotation);
        return angleDifference <= toleranceDegrees;
    }
}
