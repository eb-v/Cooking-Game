//using UnityEngine;
//using UnityEngine.InputSystem;

//public class Interact_Items : MonoBehaviour {
//    private bool Trigger_Active = false;

//    private void Awake() {
//        // Subscribe to the custom event (no params)
//        GenericEvent<Interact>.GetEvent(gameObject.GetInstanceID()).AddListener(HandleInteraction);
//    }

//    public void OnTriggerEnter(Collider other) {
//        Trigger_Active = true;
//    }

//    public void OnTriggerExit(Collider other) {
//        Trigger_Active = false;
//    }

//    // Called by the Input System when the Interact key is pressed
//    private void OnInteract(InputAction.CallbackContext context) {
//        if (Trigger_Active && context.started) {
//            Debug.Log("Input detected, broadcasting interaction event.");
//            GenericEvent<Interact>.GetEvent(gameObject.GetInstanceID()).Invoke();
//        }
//    }

//    // Called when the custom event fires
//    private void HandleInteraction() {
//        Debug.Log("The item was interacted with!");
//    }
//}
