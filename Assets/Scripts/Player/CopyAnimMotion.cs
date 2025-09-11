using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CopyAnimMotion : MonoBehaviour
{
    [SerializeField] private Transform _targetLimb;
    [SerializeField] private RagDollController playerMovement;
    private ConfigurableJoint _joint;
    private Quaternion _initialLocalRotation;
    private Quaternion _rotationOffset;
    private Quaternion lastFacing = Quaternion.identity;
    public float threshold = 0.1f;

    private void Awake()
    {
        _joint = GetComponent<ConfigurableJoint>();
        if (_joint == null)
        {
            Debug.LogError("No ConfigurableJoint found on this GameObject.");
        }

        // get initial local rotation of the joint
        _initialLocalRotation = _joint.transform.localRotation;
        _rotationOffset = Quaternion.Inverse(_initialLocalRotation) * _targetLimb.localRotation;
    }

    private void Update()
    {
        Quaternion targetLocalRotation = _targetLimb.localRotation * _rotationOffset;
        ConfigurableJointExtensions.SetTargetRotationLocal(_joint, targetLocalRotation, _initialLocalRotation);


    }
}



