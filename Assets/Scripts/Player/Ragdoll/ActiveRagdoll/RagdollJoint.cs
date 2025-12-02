using UnityEngine;

public class RagdollJoint : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private ConfigurableJoint _joint;
    [SerializeField] private string _jointName;
    public bool isConnected = true;

    public Rigidbody Rigidbody => _rigidbody;
    public ConfigurableJoint Joint => _joint;


    public void SetConfigurableJoint(ConfigurableJoint joint)
    {
        _joint = joint;
    }

    public string GetJointName()
    {
        return _jointName;
    }

    public Vector3 TargetRotationEuler
    {
        get
        {
            if (_joint != null)
            {
                return _joint.targetRotation.eulerAngles;
            }
            else
            {
                Debug.LogWarning("ConfigurableJoint is not assigned.");
                return Vector3.zero;
            }
        }
        set
        {
            if (_joint != null)
            {
                _joint.targetRotation = Quaternion.Euler(value);
            }
            else
            {
                Debug.LogWarning("ConfigurableJoint is not assigned.");
            }
        }
    }

}
