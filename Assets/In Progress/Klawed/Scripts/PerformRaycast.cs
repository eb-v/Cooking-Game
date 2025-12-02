using UnityEngine;

public class PerformRaycast : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 10f;
    [SerializeField] private float angle;
    [SerializeField] private Vector3 posOffset;

    private void Update()
    {
        RaycastForward();
    }

    private void RaycastForward()
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 direction = rotation * Vector3.forward;

        Ray ray = new Ray(transform.position + posOffset, direction);
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.green);
    }
}
