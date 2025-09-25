using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
public class CounterZone : MonoBehaviour
{
    public CounterZoneMarker marker; //assign in inspector
    public Transform snapPoint;
    private bool playerInside = false;
    private int playerCollidersInside = 0;
    [SerializeField] private GameObject counterParent;

    private void Awake()
    {
        if (counterParent == null) {
            counterParent = transform.parent.gameObject; 
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;
        if (!other.transform.root.CompareTag("Player")) return;

        playerCollidersInside++;
        if (playerCollidersInside == 1){

            GenericEvent<EnterCounter>.GetEvent(counterParent.GetInstanceID()).Invoke();
        }

        var grabLogic = root.GetComponentInChildren<GrabLogic>();
        GameObject heldObject = grabLogic?.GetGrabbedObject();

        if (heldObject != null)
            Debug.Log($"Player entered zone holding: {heldObject.name}");
        else
            Debug.Log("Player entered zone but is not holding anything.");


        Debug.Log("Invoking EnterCounter for ID " + counterParent.GetInstanceID());
    }

    private void OnTriggerExit(Collider other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;

        playerCollidersInside = Mathf.Max(0, playerCollidersInside -1);
        if (playerCollidersInside == 0){

            GenericEvent<ExitCounter>.GetEvent(counterParent.GetInstanceID()).Invoke();
            Debug.Log("ExitCounter invoked for ID " + counterParent.GetInstanceID());

        }

        var grabLogic = root.GetComponentInChildren<GrabLogic>();
        GameObject heldObject = grabLogic?.GetGrabbedObject();

        if (heldObject != null)
            Debug.Log($"Player exited zone holding: {heldObject.name}");
        else
            Debug.Log("Player exited zone but is not holding anything.");
    }
}
