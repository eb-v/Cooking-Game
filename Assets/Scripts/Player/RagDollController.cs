using UnityEngine;

public class RagDollController : MonoBehaviour
{
    [SerializeField] private Rigidbody _hips;
    [SerializeField] private ConfigurableJoint _hipJoint;
    [SerializeField] private ConfigurableJoint _leftArm;
    [SerializeField] private ConfigurableJoint _rightArm;
    [SerializeField] private GameObject _leftGrabCollider;
    [SerializeField] private GameObject _rightGrabCollider;
    public float speed = 5f;
    public float jumpForce = 5f;
    private Vector3 direction;
    private bool isGrounded;
    public float torqueMultiplier = 10f;
    private Vector3 lastDirection;

    private void Awake()
    {
        GenericEvent<OnMoveInput>.GetEvent(gameObject.GetInstanceID()).AddListener(UpdateDirection);
        GenericEvent<GroundedStatusChanged>.GetEvent(gameObject.GetInstanceID()).AddListener(UpdateIsGrounded);
        GenericEvent<OnJumpInput>.GetEvent(gameObject.GetInstanceID()).AddListener(Jump);
        GenericEvent<OnLeftGrabInput>.GetEvent(gameObject.GetInstanceID()).AddListener(OnLeftGrab);
        GenericEvent<OnLeftGrabReleased>.GetEvent(gameObject.GetInstanceID()).AddListener(OnLeftGrabRelease);
        GenericEvent<OnRightGrabInput>.GetEvent(gameObject.GetInstanceID()).AddListener(OnRightGrab);
        GenericEvent<OnRightGrabReleased>.GetEvent(gameObject.GetInstanceID()).AddListener(OnRightGrabRelease);

    }


    private void UpdateDirection(Vector2 direction)
    {
        lastDirection = this.direction;
        this.direction = new Vector3(direction.x, 0, direction.y);
    }

    private void FixedUpdate()
    {
        if (direction == Vector3.zero)
        {
            SetRotation(lastDirection);
        }
        else
        {
            SetRotation(direction);
        }

        _hips.AddForce(direction * speed);
    }

    private void UpdateIsGrounded(bool value)
    {
        isGrounded = value;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            _hips.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        } 
    }

    private void SetRotation(Vector3 moveDir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        _hipJoint.targetRotation = targetRotation;
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(_hips.rotation);

        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        Vector3 torque = axis * angle * torqueMultiplier; // adjust multiplier
        _hips.AddTorque(torque);

    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    private void OnLeftGrab()
    {
        ConfigurableJointExtensions.SetTargetRotationLocal(_leftArm, Quaternion.Euler(90, 0, 0), Quaternion.Euler(0, 0, 0));
        _leftGrabCollider.SetActive(true);
    }

    private void OnLeftGrabRelease()
    {
        ConfigurableJointExtensions.SetTargetRotationLocal(_leftArm, Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0));
        _leftGrabCollider.SetActive(false);
    }

    private void OnRightGrab()
    {
        ConfigurableJointExtensions.SetTargetRotationLocal(_rightArm, Quaternion.Euler(90, 0, 0), Quaternion.Euler(0, 0, 0));
        _rightGrabCollider.SetActive(true);
    }
    
    private void OnRightGrabRelease()
    {
        ConfigurableJointExtensions.SetTargetRotationLocal(_rightArm, Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0));
        _rightGrabCollider.SetActive(false);
    }

}



