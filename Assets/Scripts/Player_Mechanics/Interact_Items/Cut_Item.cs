using UnityEngine;
using UnityEngine.InputSystem;

public class Cut_Item : MonoBehaviour
{

    private Input_Mechanincs Cut;

    private bool isHolding = false;
    private float holdTime = 0f;            // amount of time the player held interact button

    public float requiredHoldTime = 5f;     // needs to be 5 seconds


    public GameObject Finished_Item;        // so you can add in inspector B)

    public void Awake() {
        Cut = new Input_Mechanincs();
    }

    private void OnEnable() {                 // check if player is holding
        Cut.Player.Enable();
        Cut.Player.Interact.started += StartHolding;
        Cut.Player.Interact.canceled += StopHolding;
    }

    private void OnDisable() {
        Cut.Player.Interact.started -= StartHolding;
        Cut.Player.Interact.canceled -= StopHolding;

        Cut.Player.Disable();

    }


    private void Update() {                 // while holding add time
        if (isHolding) {
            holdTime += Time.deltaTime;

            if (holdTime >= requiredHoldTime) {
                Finish_Cut();
                isHolding = false;
                holdTime = 0f;
            }
        }
    }

    private void StartHolding(InputAction.CallbackContext context) {
        isHolding = true;
        holdTime = 0f;

        Debug.Log("Started Cutting");
    }

    private void StopHolding(InputAction.CallbackContext context) { // has to hold for 5 secs consequtively
        if (isHolding) {
            isHolding = false;
            holdTime = 0f;
            Debug.Log("Stopped Cutting");
        }
    }

    private void Finish_Cut() {     // spawn the cut item
        Debug.Log("Item Cut!");

        if( Finished_Item != null) {
            Instantiate(Finished_Item, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
