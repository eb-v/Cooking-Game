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
}
