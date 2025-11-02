using UnityEngine;
using UnityEngine.Events;

public class SprinklerButton : MonoBehaviour {
    public UnityEvent<bool> OnButtonStateChanged;

    private bool isPressed = false;

    public void PressButton() {
        if (!isPressed) {
            isPressed = true;
            OnButtonStateChanged.Invoke(true);
            Debug.Log("Button Pressed");
        }
    }

    public void ReleaseButton() {
        if (isPressed) {
            isPressed = false;
            OnButtonStateChanged.Invoke(false);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.transform.root.CompareTag("Player")) {
            Debug.Log("A player triggered this button!");
            PressButton();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.transform.root.CompareTag("Player")) {
            Debug.Log("A player left this button!");
            ReleaseButton();
        }
    }
}