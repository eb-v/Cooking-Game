using UnityEngine;

[CreateAssetMenu(fileName = "ArmRotationData", menuName = "Scriptable Objects/Data/Arm Rotation Data")]
public class ArmRotationData : ScriptableObject
{
    [Header("Left Arm Rotations")]
    public Vector3 UpperLeftArmRotation;
    public Vector3 LowerLeftArmRotation;
    [Header("Right Arm Rotations")]
    public Vector3 UpperRightArmRotation;
    public Vector3 LowerRightArmRotation;   

}
