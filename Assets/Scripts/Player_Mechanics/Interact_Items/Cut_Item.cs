using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Cut_Item : MonoBehaviour {
    private InputSystem_Actions cut;

    private bool isHolding = false;
    private float holdTime = 0f;

    public float requiredHoldTime = 5f;
    public GameObject Finished_Item;

    public void Awake() {
        cut = new InputSystem_Actions();
        GenericEvent<Interact>.GetEvent(gameObject.GetInstanceID()).AddListener(OnCutItem);
    }

    public void OnEnable() {
        cut.Player.Interact.started += StartHolding;
        cut.Player.Interact.canceled += StopHolding;

        cut.Player.Enable();
    }

    public void OnDisable() {
        cut.Player.Interact.started -= StartHolding;
        cut.Player.Interact.canceled -= StopHolding;

        cut.Player.Disable();
    }

    private void StartHolding(InputAction.CallbackContext context) {
        isHolding = true;
        holdTime = 0f;

        Debug.Log("Started Cutting");
        GenericEvent<Interact>.GetEvent(gameObject.GetInstanceID()).Invoke();
    }

    private void StopHolding(InputAction.CallbackContext context) { // has to hold for 5 secs consequtively
        if (isHolding) {
            isHolding = false;
            holdTime = 0f;
            Debug.Log("Stopped Cutting");
            GenericEvent<Interact>.GetEvent(gameObject.GetInstanceID()).Invoke();
        }
    }

    private void Update() {
        if (isHolding) {
            holdTime += Time.deltaTime;

            if (holdTime >= requiredHoldTime) {
                Finish_Cut();
            }
        }
    }


    private void Finish_Cut() {
        Debug.Log("The item is finished cutting");

        if (Finished_Item != null) {
            Instantiate(Finished_Item, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
    private void OnCutItem() {
        Debug.Log("The player is cutting an item");


    }

}
