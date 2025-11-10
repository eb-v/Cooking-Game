using UnityEngine;

public class GrabScript : MonoBehaviour
{
    [HideInInspector] public bool isGrabbing = false;
    public GameObject grabbedObj;

    private void OnTriggerEnter(Collider other)
    {
        string layerName = LayerMask.LayerToName(other.gameObject.layer);
        if (layerName == "Player")
        {
            GameObject rootPlayerObj = other.transform.root.gameObject;
            if (rootPlayerObj == gameObject.transform.root.gameObject) 
            {
                return;
            }
        }

        if (layerName == LayerMask.LayerToName(LayerMask.NameToLayer("Buttons")))
            return;

        GrabSystem.GrabObject(this.gameObject, other.gameObject);
    }
}
