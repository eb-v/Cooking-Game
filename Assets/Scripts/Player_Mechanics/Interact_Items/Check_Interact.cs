using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour {
    private InputSystem_Actions input;

    private void Awake() {
        input = new InputSystem_Actions();
        GenericEvent<Interact>.GetEvent(gameObject.GetInstanceID()).AddListener(OnInteractEvent);
    }

    private void OnEnable() {
        input.Player.Enable();
        input.Player.Interact.performed += Interact_Item;
    }

    private void OnDisable() {
        input.Player.Interact.performed -= Interact_Item;
        input.Player.Disable();
    }

    private void Interact_Item(InputAction.CallbackContext context) {
        GenericEvent<Interact>.GetEvent(gameObject.GetInstanceID()).Invoke();
    }

    private void OnInteractEvent() {
        Debug.Log("The player interacted with an item");
    }
}
