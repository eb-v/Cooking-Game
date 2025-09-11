using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 direction;
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    private Rigidbody _rb;

    private void Awake()
    {
        GenericEvent<OnMoveInput>.GetEvent(gameObject.GetInstanceID()).AddListener(UpdateDirection);
        GenericEvent<OnJumpInput>.GetEvent(gameObject.GetInstanceID()).AddListener(Jump);

        _rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        UpdateRotation();
    }

    private void Move()
    {
        _rb.linearVelocity = new Vector3(direction.x * speed, _rb.linearVelocity.y, direction.z * speed);
    }

    private void UpdateDirection(Vector2 direction)
    {
        this.direction = new Vector3(direction.x, 0, direction.y);
    }

    private void UpdateRotation()
    {
        transform.forward = Vector3.Slerp(transform.forward, direction, rotationSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector3 (_rb.linearVelocity.x, jumpForce, _rb.linearVelocity.z);
    }

    public Vector3 GetDirection()
    {
        return direction;
    }
}
