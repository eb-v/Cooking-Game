using UnityEngine;

public class SnapJointTesting : MonoBehaviour
{
    [SerializeField] private ConfigurableJoint UpperRightArm;
    [SerializeField] private ConfigurableJoint LowerRightArm;
    [SerializeField] private ConfigurableJoint UpperLeftArm;
    [SerializeField] private ConfigurableJoint LowerLeftArm;


    [SerializeField] private ArmRotationData armRotationData;
    void Update()
    {
        //PoseHelper.SnapJointToRotation(UpperRightArm, Quaternion.Euler(armRotationData.UpperRightArmRotation));
        //PoseHelper.SnapJointToRotation(LowerRightArm, Quaternion.Euler(armRotationData.LowerRightArmRotation));
        //PoseHelper.SnapJointToRotation(UpperLeftArm, Quaternion.Euler(armRotationData.UpperLeftArmRotation));
        //PoseHelper.SnapJointToRotation(LowerLeftArm, Quaternion.Euler(armRotationData.LowerLeftArmRotation));

        //Debug.LogError("Snapped Joints");
    }

    
}
