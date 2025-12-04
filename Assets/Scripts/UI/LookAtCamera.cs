using System.Collections;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform _camera;

    private void LateUpdate()
    {
        if (_camera == null)
        {
            StartCoroutine(GetCamera());
            return;
        }

        Transform cam = _camera;

        // Get the camera position, but align its Y (height) to this object
        Vector3 lookPos = cam.position;
        lookPos.x = transform.position.x;

        // Make the object look at that adjusted position
        transform.LookAt(lookPos);
    }

    private IEnumerator GetCamera()
    {
        yield return new WaitForSeconds(0.1f);
        _camera = Camera.main.transform;
    }
}
