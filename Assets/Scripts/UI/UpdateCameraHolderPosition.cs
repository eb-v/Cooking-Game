using UnityEngine;

public class UpdateCameraHolderPosition : MonoBehaviour
{
    [SerializeField] private Transform centerOfMass;

    public Vector3 offset;


    private void FixedUpdate()
    {
        Vector3 targetPosition = centerOfMass.position + offset;
        gameObject.transform.position = targetPosition;
    }
}
