using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILookAtCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return; 
        
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
