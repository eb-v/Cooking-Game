using UnityEngine;

public class ApplyPositionOffset : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        transform.localPosition = initialPosition + offset;
    }
}
