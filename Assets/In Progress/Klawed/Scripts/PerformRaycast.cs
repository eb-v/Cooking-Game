using UnityEngine;

public class PerformRaycast : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 10f;

    private void Update()
    {
        RaycastForward();
    }

    private void RaycastForward()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.green);
    }
}
