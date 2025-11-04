using UnityEngine;

public class DroneHover : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 counterVelocity;
    public Vector3 currentVelocity;

    private void FixedUpdate()
    {
        rb.AddForce(counterVelocity * rb.mass);
        currentVelocity = rb.linearVelocity;
    }


}
