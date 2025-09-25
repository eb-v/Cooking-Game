using UnityEngine;

public class RotationLimiter : MonoBehaviour
{
    public float min;
    public float max;

    void Update()
    {
        Vector3 currentRotation = transform.localEulerAngles;

        float xRotation = currentRotation.x;

        currentRotation.x = Mathf.Clamp(xRotation, min, max);

        transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z);
    }
}
