using UnityEngine;

public class DroneHover : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 currentVelocity;
    public float speed = 5f;

    private void FixedUpdate()
    {
        MoveForward();
        currentVelocity = rb.linearVelocity;
    }

    private void MoveForward()
    {
        rb.linearVelocity = Vector3.up * speed;
    }

}
