using UnityEngine;

public class Interactable_Item : MonoBehaviour{

    private void Awake() {
        Debug.Log($"{gameObject.name} Interactable_Item active");
    }

    private void OnTriggerEnter(Collider other) {
        GameObject root = other.transform.root.gameObject;

        if (root.CompareTag("Player")) {
            CheckInteract ci = root.GetComponent<CheckInteract>();
            //if (ci != null) ci.SetCurrentInteractable(this);
            //Debug.Log($"Trigger Enter: {root.name} entered {gameObject.name}");

        }
    }

    private void OnTriggerExit(Collider other) {
        GameObject root = other.transform.root.gameObject;
        if (root.CompareTag("Player")) {
            CheckInteract ci = root.GetComponent<CheckInteract>();
            //if (ci != null) ci.ClearCurrentInteractable(this);
        }
    }

    public void Interact() {
        //Debug.Log("THE CHEESE TOUCH");
        // Start your skill check here
    }

    public void StopInteract() {
        //Debug.Log("THE NOOOOT CHEESE TOUCH");
    }
}
