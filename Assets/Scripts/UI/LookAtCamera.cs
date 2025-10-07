using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void LateUpdate()
    {

        Transform cam = Camera.main.transform;

        // Get the camera position, but align its Y (height) to this object
        Vector3 lookPos = cam.position;
        lookPos.x = transform.position.x;

        // Make the object look at that adjusted position
        transform.LookAt(lookPos);
    }
}
