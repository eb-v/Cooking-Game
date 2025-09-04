using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 direction;
    public float speed = 5f;
    public float rotationSpeed = 10f;

    private void Awake()
    {
        GenericEvent<OnMoveInput>.GetEvent(gameObject.GetInstanceID()).AddListener(UpdateDirection);
    }

    private void Update()
    {
        Move();
        UpdateRotation();
    }

    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void UpdateDirection(Vector2 direction)
    {
        this.direction = new Vector3(direction.x, 0, direction.y);
    }

    private void UpdateRotation()
    {
        transform.forward = Vector3.Slerp(transform.forward, direction, rotationSpeed * Time.deltaTime);
    }
}
