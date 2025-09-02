using UnityEngine;

public class Movement : MonoBehaviour
{
    Vector3 direction;
    public float speed = 5f;

    private void Awake()
    {
        GenericEvent<OnMoveInput>.GetEvent(gameObject.GetInstanceID()).AddListener(UpdateDirection);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void UpdateDirection(Vector2 direction)
    {
        this.direction = new Vector3(direction.x, 0, direction.y);
    }
}
