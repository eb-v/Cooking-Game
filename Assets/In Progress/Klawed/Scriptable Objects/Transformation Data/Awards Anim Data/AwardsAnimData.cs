using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AwardsAnimData", menuName = "Animation Data/AwardsAnimData")]
public class AwardsAnimData : ScriptableObject
{
    [SerializeField] private Vector3 cameraWideshotOffset;
    [SerializeField] private List<Vector3> startingPositions;
    [SerializeField] private List<Vector3> startingRotations;

    [SerializeField] private List<Vector3> cameraWideShotPositions;

    public Vector3 CameraWideshotOffset => cameraWideshotOffset;

}


