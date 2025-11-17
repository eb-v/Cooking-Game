using Unity.VisualScripting;
using UnityEngine;

public class SaveGrabData : MonoBehaviour
{
    [SerializeField] private GrabData grabData;
    [SerializeField] private RagdollController rc;
    [SerializeField] private Transform objToGrab;
    [SerializeField] private Transform parent;


    public void SaveGrabDataFunction()
    {
        grabData.leftArmPose.upperArmRot = rc.RagdollDict["UpperLeftArm"].Joint.targetRotation;
        grabData.leftArmPose.lowerArmRot = rc.RagdollDict["LowerLeftArm"].Joint.targetRotation;
        grabData.rightArmPose.upperArmRot = rc.RagdollDict["UpperRightArm"].Joint.targetRotation;
        grabData.rightArmPose.lowerArmRot = rc.RagdollDict["LowerRightArm"].Joint.targetRotation;

        grabData.position = parent.InverseTransformPoint(objToGrab.position);
        grabData.rotation = Quaternion.Inverse(parent.rotation) * objToGrab.rotation;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(grabData);
#endif
        Debug.Log("Pose Saved to PoseData Scriptable Object");
    }
}
