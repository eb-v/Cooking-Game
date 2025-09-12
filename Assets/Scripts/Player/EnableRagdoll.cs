using UnityEngine;

public class EnableRagdoll : MonoBehaviour
{
    private CopyAnimMotion[] _CopyAnims;
    private ConfigurableJoint[] _joints;
    [SerializeField] private Rigidbody _hipsRb;

    private void Awake()
    {
        _CopyAnims = gameObject.GetComponentsInChildren<CopyAnimMotion>();
        _joints = gameObject.GetComponentsInChildren<ConfigurableJoint>();
    }

    private void OnEnable()
    {
        gameObject.GetComponent<RagDollController>().enabled = false;

        foreach (var copyAnim in _CopyAnims)
        {
            copyAnim.enabled = false;
        }

        foreach (var joint in _joints)
        {
            JointDrive drive = joint.angularXDrive;
            drive.positionSpring = 0;
            joint.angularXDrive = drive;
        }

        _hipsRb.constraints &= ~RigidbodyConstraints.FreezePositionX;
        _hipsRb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
    }

    private void OnDisable()
    {
        gameObject.GetComponent<RagDollController>().enabled = true;
        foreach (var copyAnim in _CopyAnims)
        {
            copyAnim.enabled = true;
        }

        foreach (var joint in _joints)
        {
            JointDrive drive = joint.angularXDrive;
            drive.positionSpring = 500; // or whatever value you want
            joint.angularXDrive = drive;
        }

        _hipsRb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    }

}
