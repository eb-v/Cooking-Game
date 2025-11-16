using UnityEngine;
using UnityEngine.InputSystem;

public class CheckInteract : MonoBehaviour {
    private IInteractable currentInteractable;
    private PlayerController playerController;

    private void Awake() {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
            Debug.LogWarning("PlayerController not found on this GameObject.");
    }
    public void SetCurrentInteractable(IInteractable interactable) {
        currentInteractable = interactable;
        Debug.Log("Current interactable set to: " + ((MonoBehaviour)interactable).gameObject.name);
    }

    public void ClearCurrentInteractable(IInteractable interactable) {
        if (currentInteractable == interactable) {
            Debug.Log("Current interactable cleared: " + ((MonoBehaviour)interactable).gameObject.name);
            currentInteractable = null;
        }
    }
    private void Update() {
        if (currentInteractable != null && playerController.IsInteractPressed) {
           // currentInteractable.Interact();
        } else if(currentInteractable != null && !playerController.IsInteractPressed){
           // currentInteractable.StopInteract();
        }
    }
}
