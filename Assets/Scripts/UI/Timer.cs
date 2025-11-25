using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {
    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTime = 300f;

    private float timeRemaining;
    private bool gameOver = false;
    private bool timerOn = true;

    private bool finalThreeSFXStarted = false;

    private void Start() {
        timeRemaining = startTime;
        finalThreeSFXStarted = false;
        DisplayTime(timeRemaining);
    }

    private void Update() {
        if (!timerOn || gameOver || Time.timeScale <= 0)
            return;

        if (timeRemaining > 0) {
            timeRemaining -= Time.deltaTime;

            float clamped = Mathf.Max(0f, timeRemaining);
            DisplayTime(clamped);

            GenericEvent<GameTimeUpdatedEvent>.GetEvent("Timer").Invoke(clamped);

            if (!finalThreeSFXStarted && clamped > 0f && clamped <= 3f) {
                finalThreeSFXStarted = true;
                PlayFinalThreeSecondsSFX();
            }
        } else {
            timeRemaining = 0;
            DisplayTime(0);
            gameOver = true;
            timerOn = false;

            GenericEvent<GameOverEvent>.GetEvent("Timer").Invoke();
            GenericEvent<ShowEndGameUIEvent>.GetEvent("EndGameUI").Invoke();
            Debug.Log("starting end game ui event");

            Debug.Log("[Timer] Game over event invoked.");
        }
    }

    private void DisplayTime(float timeToDisplay) {
        timeToDisplay = Mathf.Max(0, timeToDisplay);
        timeToDisplay = Mathf.Ceil(timeToDisplay);  // round up

        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StartTimer() {
        timerOn = true;
        if (timeRemaining > 3f) finalThreeSFXStarted = false;
    }

    public void StopTimer() => timerOn = false;

    private void PlayFinalThreeSecondsSFX() {
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX("TimerEnd");
        } else {
            Debug.LogWarning("[Timer] Wanted to play TimerEnd SFX, but AudioManager.Instance is null.");
        }
    }
}
