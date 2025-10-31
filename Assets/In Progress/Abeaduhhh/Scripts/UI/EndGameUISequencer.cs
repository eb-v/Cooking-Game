using UnityEngine;
using System.Collections;
using TMPro;

public class EndGameUISequencer : MonoBehaviour {
    [Header("Overlay & Button Springs")]
    public SpringAPI overlaySpring; // Main canvas spring (like levelCompleteSpring)
    public SpringAPI replayButtonSpring;
    public SpringAPI mainMenuButtonSpring;

    [Header("Button GameObjects")]
    public GameObject replayButtonGO;
    public GameObject mainMenuButtonGO;

    [Header("Sequence Timing")]
    public float delayBeforeButtons = 0.5f;
    public float delayBetweenButtons = 0.2f;
    public float delayBeforeOverlay = 0.1f;

    [Header("Testing")]
    public bool testOnStart = false;

    private void Start() {
        // Hide buttons initially
        if (replayButtonGO != null) replayButtonGO.SetActive(false);
        if (mainMenuButtonGO != null) mainMenuButtonGO.SetActive(false);

        // Hide overlay initially
        if (overlaySpring != null) overlaySpring.ResetSpring();

        if (testOnStart) {
            PlayEndGameUISequence();
        }
    }

    public void PlayEndGameUISequence() {
        StartCoroutine(RunEndGameSequence());
    }

    private IEnumerator RunEndGameSequence() {
        // --- OVERLAY POP IN ---
        if (overlaySpring != null) {
            overlaySpring.gameObject.SetActive(true);
            overlaySpring.SetGoalValue(1f);
            overlaySpring.NudgeSpringVelocity();
            yield return new WaitForSecondsRealtime(delayBeforeOverlay);
        }

        // --- BUTTONS POP IN SEQUENTIALLY ---
        if (replayButtonGO != null && replayButtonSpring != null) {
            replayButtonGO.SetActive(true);
            replayButtonSpring.SetGoalValue(1f);
            replayButtonSpring.NudgeSpringVelocity();
            yield return new WaitForSecondsRealtime(delayBetweenButtons);
        }

        if (mainMenuButtonGO != null && mainMenuButtonSpring != null) {
            mainMenuButtonGO.SetActive(true);
            mainMenuButtonSpring.SetGoalValue(1f);
            mainMenuButtonSpring.NudgeSpringVelocity();
        }
    }

    // Optional: Reset everything for replay
    public void ResetEndGameUI() {
        if (overlaySpring != null) overlaySpring.ResetSpring();
        if (replayButtonSpring != null) replayButtonSpring.ResetSpring();
        if (mainMenuButtonSpring != null) mainMenuButtonSpring.ResetSpring();

        if (replayButtonGO != null) replayButtonGO.SetActive(false);
        if (mainMenuButtonGO != null) mainMenuButtonGO.SetActive(false);
        if (overlaySpring != null) overlaySpring.gameObject.SetActive(false);
    }
}
