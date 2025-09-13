using UnityEngine;

public class JointManager : MonoBehaviour
{
    [SerializeField] private ConfigurableJoint _headJoint;
    [SerializeField] private ConfigurableJoint _leftUpperArm;
    [SerializeField] private ConfigurableJoint _leftLowerArm;
    [SerializeField] private ConfigurableJoint _rightUpperArm;
    [SerializeField] private ConfigurableJoint _rightLowerArm;
    [SerializeField] private ConfigurableJoint _chest;
    [SerializeField] private ConfigurableJoint _pelvis;
    [SerializeField] private ConfigurableJoint _leftUpperLeg;
    [SerializeField] private ConfigurableJoint _leftLowerLeg;
    [SerializeField] private ConfigurableJoint _rightUpperLeg;
    [SerializeField] private ConfigurableJoint _rightLowerLeg;
    [SerializeField] private ConfigurableJoint _leftFoot;
    [SerializeField] private ConfigurableJoint _rightFoot;



    public void SetTargetRotation(ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation)
    {
        ConfigurableJointExtensions.SetTargetRotationLocal(joint, targetLocalRotation, startLocalRotation);
    }







}
