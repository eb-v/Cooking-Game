using UnityEngine;

[CreateAssetMenu(fileName = "GrabData", menuName = "Scriptable Objects/Data/GrabData")]
public class GrabData : ScriptableObject
{
    public ArmPose leftArmPose;
    public ArmPose rightArmPose;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class ArmPose
{
    public Quaternion upperArmRot;
    public Quaternion lowerArmRot;
}
