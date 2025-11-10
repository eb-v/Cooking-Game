using UnityEngine;

// this script does raycast detection from players head for re attaching other players limbs
// should only hit other players
public class BodyRayCast : MonoBehaviour
{
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private float interactionRange = 6f;
    private int layerMask;
    private GameObject currentlyLookingAt;

    // Update is called once per frame
    void FixedUpdate()
    {
        RunRayCast();
    }

    private void RunRayCast()
    {
        Ray ray = new Ray(transform.position, centerOfMass.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, layerMask) && hit.collider.transform.root.name != gameObject.transform.root.name)
        {
            currentlyLookingAt = hit.collider.transform.root.gameObject;
            
            Debug.Log("Looking at " + currentlyLookingAt.name);
            Debug.DrawRay(transform.position, centerOfMass.forward * interactionRange, Color.green);
        }
        else
        {
            Debug.DrawRay(transform.position, centerOfMass.forward * interactionRange, Color.red);
        }
    }
}
