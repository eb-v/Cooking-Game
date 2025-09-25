using UnityEngine;

public class RagdollJoint : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private ConfigurableJoint _joint;

    public Rigidbody Rigidbody => _rigidbody;
    public ConfigurableJoint Joint => _joint;
}
