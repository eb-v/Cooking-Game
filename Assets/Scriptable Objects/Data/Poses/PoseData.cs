using UnityEngine;

[CreateAssetMenu(fileName = "PoseData", menuName = "Scriptable Objects/Data/PoseData")]
public class PoseData : ScriptableObject
{
    public UDictionary<string, Quaternion> poseJointTargetRotations;

    public UDictionary<string, Quaternion> poseWorldRotations;
}
