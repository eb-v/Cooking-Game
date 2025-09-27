using UnityEngine;

public class GrabbedObject : MonoBehaviour
{
    private GameObject grabbedObject;

    public void SetGrabbedObject(GameObject obj)
    {
        grabbedObject = obj;
    }

    public GameObject GetGrabbedObject()
    {
        return grabbedObject;
    }
}
