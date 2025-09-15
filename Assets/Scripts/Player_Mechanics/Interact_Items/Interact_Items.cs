using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact_Items : MonoBehaviour {
    private bool Trigger_Active = false;
    //private GameObject In_Range_Object;
    private Input_Mechanincs Interact; // Reference to the input system made in Player_Inputs
    //private GameObject Item_In_Hand;

    private void Awake() {
        Interact = new Input_Mechanincs(); // create new instance of the input
    }

    private void OnEnable() {
        Interact.Player.Enable(); // allow the player to interact
        Interact.Player.Interact.performed += OnInteract;
    }

    private void OnDisable() {
        Interact.Player.Interact.performed -= OnInteract; // stop the player from interacting
        Interact.Player.Disable();
    }

    public void OnTriggerEnter(Collider other) { // currently works for any object that walks into the trigger need to change.
        Trigger_Active = true;
    }


    public void OnTriggerExit(Collider other) {
        Trigger_Active = false;
        //In_Range_Object = null;
    }

    private void OnInteract(InputAction.CallbackContext context) {
        if (Trigger_Active) {
            Debug.Log("The item was iteracted with :(");
        }

        //if (Item_In_Hand == null && In_Range_Object != null && Trigger_Active) {
        //    Hold_The_Item(In_Range_Object);
        }
        //if (triggerActive)  // check if player presses e to interact
        //    do_something();
    }

    //private void Hold_The_Item(GameObject Item) {
    //    Item_In_Hand = Item;
    //    Debug.Log("The item is now HELD");
    //}

    //private void Drop_The_Item(GameObject Item) {
    //    Item_In_Hand = null;
    //    Debug.Log("The item is Dropped");
    //}

    //private void do_something() {
    //    Debug.Log("The Player interacted with the object");
    //}
//}