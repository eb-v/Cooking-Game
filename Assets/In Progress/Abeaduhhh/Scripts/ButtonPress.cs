using UnityEngine;
using UnityEngine.Events;

public class SprinklerButton : MonoBehaviour {
    [Header("Events")]
    public UnityEvent<bool> OnButtonStateChanged;

    [Header("Button Settings")]
    [Tooltip("How long before the button resets (pops back up) after being pressed.")]
    public float resetDelay = 0.5f;

    private bool isPressed = false;

    public void PressButton() {
        if (isPressed) return;

        isPressed = true;
        OnButtonStateChanged.Invoke(true);
        Debug.Log("Button Pressed");
        Invoke(nameof(ReleaseButton), resetDelay);
    }

    public void ReleaseButton() {
        if (!isPressed) return;

        isPressed = false;
        OnButtonStateChanged.Invoke(false);
        Debug.Log("Button Released");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.root.CompareTag("Player")) {
            Debug.Log("A player triggered this button!");
            PressButton();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.root.CompareTag("Player")) {
            Debug.Log("A player left this button!");
            // ReleaseButton();
        }
    }
}