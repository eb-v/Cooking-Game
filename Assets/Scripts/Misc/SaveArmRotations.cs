using UnityEngine;

public class SaveArmRotations : MonoBehaviour
{
    [SerializeField] private ArmRotationData armRotation;
    [SerializeField] private RagdollController rc;

    public void SaveArmRotationsFunction()
    {
        // get the arm joints
        Quaternion leftArmRotation = rc.RagdollDict["UpperLeftArm"].Joint.transform.rotation;
        Quaternion rightArmRotation = rc.RagdollDict["UpperRightArm"].Joint.transform.rotation;
        Quaternion leftForearmRotation = rc.RagdollDict["LowerLeftArm"].Joint.transform.rotation;
        Quaternion rightForearmRotation = rc.RagdollDict["LowerRightArm"].Joint.transform.rotation;

        // save the rotations
        armRotation.UpperLeftArmRotation = leftArmRotation.eulerAngles;
        armRotation.UpperRightArmRotation = rightArmRotation.eulerAngles;
        armRotation.LowerLeftArmRotation = leftForearmRotation.eulerAngles;
        armRotation.LowerRightArmRotation = rightForearmRotation.eulerAngles;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(armRotation);
#endif
        Debug.Log("Arm rotations saved to GrabData.");
    }
}
