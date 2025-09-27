using UnityEngine;
using UnityEngine.InputSystem;

public class SnapLogic : MonoBehaviour
{
    [SerializeField] private Transform snapPoint;
    [SerializeField] private InputActionReference snapAction;
    [SerializeField] private GameObject counterParent; 

    private GrabLogic playerGrabLogic;
    private GameObject counterObj;
    private bool objPresent = false;
    private bool isSnapped = false;

    private void Awake()
    {
        if (counterParent == null) {
            counterParent = gameObject; 
        }

        GenericEvent<EnterCounter>.GetEvent(counterParent.name).AddListener(OnEnterCounterZone);

        GenericEvent<ExitCounter>.GetEvent(counterParent.name).AddListener(OnExitCounterZone);

        Debug.Log("SnapLogic Awake: zone listeners added.");
    }

    private void Start()
    {
        SnapPoint snapPointScript = GetComponent<SnapPoint>();
        if (snapPointScript != null)
        {
            snapPoint = snapPointScript.GetSnapPos();
            Debug.Log($"SnapLogic got snap point at {snapPoint.position}");
        }
    }


    private void OnEnterCounterZone()   //onEnable
    {
        Debug.Log("Player entered counter zone → enabling snap input.");

        GameObject player = GameObject.FindWithTag("Player"); 
        if (player != null){
            playerGrabLogic = player.GetComponentInChildren<GrabLogic>();
        }

        if (snapAction != null)
        {
            snapAction.action.performed += OnSnapAction;
        }
    }

    private void OnExitCounterZone()  //onDisable
    {
        Debug.Log("Player exited counter zone → disabling snap input.");

        if (snapAction != null)
        {
            snapAction.action.performed -= OnSnapAction;
        }
        playerGrabLogic = null;
    }

    private void OnSnapAction(InputAction.CallbackContext context)
    {
        Debug.Log("Snap action performed inside counter zone.");

        if (!objPresent)
            SnapObject();
        else
            UnsnapObject();
    }

    private void SnapObject()
    {
        Debug.Log("SnapObject called.");

        if (playerGrabLogic == null) {
            Debug.LogWarning("No player GrabLogic found, cannot snap.");
            return;
        }

        GameObject heldObject = playerGrabLogic.GetGrabbedObject();
        if (heldObject == null)
        {
            Debug.Log("Player is not holding any object to snap.");
            return;
        }
        
        counterObj = heldObject;

        GenericEvent<OnGrabReleased>.GetEvent(gameObject.name).Invoke();
        Rigidbody rb = counterObj.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
        }

        //position min y of counterObj to snapPos
        Renderer rend = counterObj.GetComponent<Renderer>();
        float offset = rend.bounds.min.y - counterObj.transform.position.y; 
        Vector3 surfacePos = snapPoint.position;
        surfacePos.y -= offset;  
        counterObj.transform.position = surfacePos;
        counterObj.transform.rotation = snapPoint.rotation;

        objPresent = true;
        isSnapped = true;

        Debug.Log($"Snapped and released held object: {counterObj?.name}");
    }

    private void UnsnapObject()
    {
        Debug.Log("UnsnapObject called.");
        if (counterObj != null)
        {
            counterObj.GetComponent<Rigidbody>().isKinematic = false;
            //move it away for testing
            counterObj.transform.position += Vector3.up;
            counterObj.transform.rotation = snapPoint.rotation;
            counterObj = null;
        }

        objPresent = false;
        isSnapped = false;

        Debug.Log($"Unsnapped: {counterObj?.name}");
    }
}
