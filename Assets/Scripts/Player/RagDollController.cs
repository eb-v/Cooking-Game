using UnityEngine;

public class RagDollController : MonoBehaviour
{
    [SerializeField] private Rigidbody _hips;
    [SerializeField] private ConfigurableJoint _hipJoint;
    public float speed = 5f;
    public float jumpForce = 5f;
    private Vector3 direction;
    private bool isGrounded;
    public float torqueMultiplier = 10f;

    private void Awake()
    {
        GenericEvent<OnMoveInput>.GetEvent(gameObject.GetInstanceID()).AddListener(UpdateDirection);
        GenericEvent<GroundedStatusChanged>.GetEvent(gameObject.GetInstanceID()).AddListener(UpdateIsGrounded);
        GenericEvent<OnJumpInput>.GetEvent(gameObject.GetInstanceID()).AddListener(Jump);
    }


    private void UpdateDirection(Vector2 direction)
    {
        this.direction = new Vector3(direction.x, 0, direction.y);
    }

    private void FixedUpdate()
    {
        SetRotation();
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

    private void SetRotation()
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-direction);
            _hipJoint.targetRotation = Quaternion.Inverse(targetRotation);
            Quaternion deltaRotation = targetRotation * Quaternion.Inverse(_hips.rotation);

            deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;

            Vector3 torque = axis * angle * torqueMultiplier; // adjust multiplier
            _hips.AddTorque(torque);

            
        }
    }

}



