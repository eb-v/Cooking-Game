using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillCheckManager : MonoBehaviour, IInteractable {
    [Header("UI & Visuals")]
    [SerializeField] private GameObject skillCheckUI;
    [SerializeField] private Renderer skillCheckSphere;

    [Header("Skill Check Settings")]
    [Tooltip("Seconds for color toggle interval")]
    [SerializeField] private float toggleInterval = 1f;

    [Tooltip("Maximum duration of the skill check in seconds")]
    [SerializeField] private float skillCheckDuration = 5f;

    [Header("Input Handling")]
    [Tooltip("If true, the skill check will wait for the player to release the interact button before accepting an answer.")]
    [SerializeField] private bool requireReleaseBeforeAcceptingInput = true;

    [Tooltip("Fallback grace period (seconds) to ignore input right after skill check starts.")]
    [SerializeField] private float inputGracePeriod = 0.15f;

    public event Action OnSkillCheckStarted;
    public event Action<bool> OnSkillCheckFinished;

    private bool skillCheckActive = false;
    private bool ignoreFirstPress = false;
    private bool inputReleased = true;
    private bool isGreen = false;
    private float toggleTimer = 0f;
    private float durationTimer = 0f;
    private float inputGraceTimer = 0f;

    private void Awake() {
        if (skillCheckUI != null)
            skillCheckUI.SetActive(false);

        if (skillCheckSphere == null)
            skillCheckSphere = GetComponent<Renderer>();

        if (skillCheckSphere != null)
            skillCheckSphere.material.color = Color.red;
    }

    private void Update() {
        if (!skillCheckActive) return;

        inputGraceTimer += Time.deltaTime;

        toggleTimer += Time.deltaTime;
        if (toggleTimer >= toggleInterval) {
            ToggleColor();
            toggleTimer = 0f;
        }

        durationTimer += Time.deltaTime;
        if (durationTimer >= skillCheckDuration) {
            EndSkillCheck(false);
        }
    }

    private void ToggleColor() {
        if (skillCheckSphere == null) return;

        isGreen = !isGreen;
        skillCheckSphere.material.color = isGreen ? Color.green : Color.red;
    }

    public void Interact() {
        if (!skillCheckActive) return;

        if (ignoreFirstPress) {
            ignoreFirstPress = false;
            Debug.Log("SkillCheckManager: Ignored first press (start input).");
            return;
        }

        if (requireReleaseBeforeAcceptingInput && !inputReleased) {
            if (inputGraceTimer < inputGracePeriod) {
                Debug.Log("SkillCheckManager: Waiting for release");
                return;
            } else {
                Debug.Log("SkillCheckManager: Button still held; waiting for release.");
                return;
            }
        }

        Debug.Log($"SkillCheckManager: Player pressed interact ({(isGreen ? "GREEN" : "RED")})");

        if (isGreen) EndSkillCheck(true);
        else EndSkillCheck(false);
    }

    public void StopInteract() {
        inputReleased = true;
        Debug.Log("SkillCheckManager: Interact released (StopInteract called).");
    }

    public void StartSkillCheck() {
        if (skillCheckActive) return;

        skillCheckActive = true;
        isGreen = false;
        toggleTimer = 0f;
        durationTimer = 0f;

        inputGraceTimer = 0f;
        inputReleased = false;
        ignoreFirstPress = true;

        if (skillCheckUI != null) skillCheckUI.SetActive(true);
        if (skillCheckSphere != null) skillCheckSphere.material.color = Color.red;

        OnSkillCheckStarted?.Invoke();
        Debug.Log("SkillCheckManager: Skill check started!");
    }

    private void EndSkillCheck(bool success) {
        if (!skillCheckActive) return;

        skillCheckActive = false;

        if (skillCheckUI != null) skillCheckUI.SetActive(false);
        if (skillCheckSphere != null) skillCheckSphere.material.color = Color.red;

        inputReleased = true;
        ignoreFirstPress = false;
        inputGraceTimer = 0f;

        Debug.Log($"SkillCheckManager: Skill check ended ({(success ? "SUCCESS" : "FAIL")})");
        OnSkillCheckFinished?.Invoke(success);
    }

    public bool IsActive() => skillCheckActive;
}
