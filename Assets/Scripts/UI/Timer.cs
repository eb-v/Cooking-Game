using UnityEngine;
using TMPro;
using System.Collections;

public class Timer : MonoBehaviour {
    [Header("Timer Settings")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float startTime = 300f; // 5 minutes

    [Header("UI References")]
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] GameObject endGameCanvas;
    [SerializeField] GameObject gameOverButtons;
    [SerializeField] GameObject starGroup;
    [SerializeField] TextMeshProUGUI scoreAmountText;
    [SerializeField] GameObject background;
    [SerializeField] GameObject pointsObject;
    [SerializeField] GameObject timerObject;






    [Header("End Game Systems")]
    [SerializeField] EndGameAwards endGameAwards;

    [Header("Timing")]
    [SerializeField] private float awardDelay = 0.5f;
    [SerializeField] private float starPopDelay = 0.3f;

    private float timeRemaining;
    private bool gameOver = false;
    private bool hasStarted = false;
    private float startDelay = 0f;

    void Start() {
        timeRemaining = startTime;

        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
        if (endGameCanvas != null) endGameCanvas.SetActive(false);
        if (gameOverButtons != null) gameOverButtons.SetActive(false);

        DisplayTime(timeRemaining);
        EnableAllPlayerMovement();
    }

    private void EnableAllPlayerMovement() {
        if (PlayerManager.Instance == null) return;

        foreach (var player in PlayerManager.Instance.GetAllPlayers()) {
            var ragdoll = player.GetComponent<RagdollController>();
            if (ragdoll != null)
                ragdoll.TurnMovementOn();
        }
    }

    void Update() {
        if (gameOver || Time.timeScale <= 0) return;

        if (!hasStarted) {
            hasStarted = true;
            startDelay = 0f;
        }

        if (startDelay < 1f) {
            startDelay += Time.deltaTime;
            DisplayTime(timeRemaining);
            return;
        }

        if (timeRemaining > 0) {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        } else {
            timeRemaining = 0;
            timerText.text = "00:00";
            gameOver = true;
            Time.timeScale = 0f;
            StartCoroutine(HandleGameOver());
        }
    }

    private void DisplayTime(float timeToDisplay) {
        timeToDisplay = Mathf.Max(0, timeToDisplay);
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private int CalculateStars(int score) {
        if (score >= 0) return 3;
        //if (score >= 600) return 2;
        //if (score >= 300) return 1;

        return 0;
    }

    private IEnumerator ShowStars(int starsEarned) {
        if (starGroup == null) yield break;

        starGroup.SetActive(true);

        // Deactivate all stars first
        for (int i = 0; i < starGroup.transform.childCount; i++)
            starGroup.transform.GetChild(i).gameObject.SetActive(false);

        // Small delay before stars start popping
        yield return new WaitForSecondsRealtime(0.75f);

        for (int i = 0; i < starsEarned && i < starGroup.transform.childCount; i++) {
            var star = starGroup.transform.GetChild(i).gameObject;
            star.SetActive(true);  

            // Wait 0.5 seconds before triggering the spring
            yield return new WaitForSecondsRealtime(0f);

            var spring = star.GetComponent<SpringAPI>();
            spring?.PlaySpring();

            // Wait a short delay between stars popping
            yield return new WaitForSecondsRealtime(starPopDelay);
        }
    }
    private IEnumerator HandleGameOver() {
        int delivered = PointManager.Instance != null ? PointManager.Instance.GetDeliveredCount() : 0;
        int finalScore = delivered * 300;
        int starsEarned = CalculateStars(finalScore);

        // --- STEP 1: Show game over canvas ---
        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        if (background != null) background.SetActive(true);

        // Animate score counting up
        if (scoreAmountText != null) {
            scoreAmountText.gameObject.SetActive(true);
            yield return StartCoroutine(AnimateScoreCount(finalScore, 1.0f));
        }

        yield return ShowStars(starsEarned);

        yield return new WaitForSecondsRealtime(1.5f);

        //  Reverse GameOverCanvas and Stars
        if (gameOverCanvas != null) {
            foreach (Transform child in gameOverCanvas.transform) {
                var springs = child.GetComponentsInChildren<SpringAPI>(true);
                foreach (var spring in springs) {
                    spring.SetGoalValue(0f);
                    spring.NudgeSpringVelocity();
                }
            }
        }

        if (starGroup != null) {
            for (int i = 0; i < starGroup.transform.childCount; i++) {
                var star = starGroup.transform.GetChild(i).gameObject;
                var spring = star.GetComponent<SpringAPI>();
                spring?.SetGoalValue(0f);
                spring?.NudgeSpringVelocity();
            }
        }

        yield return new WaitForSecondsRealtime(0.75f);

        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
        if (starGroup != null) starGroup.SetActive(false);

        if (endGameCanvas != null) {
            endGameCanvas.SetActive(true);

            yield return new WaitForSecondsRealtime(0.3f);
        }

        if (endGameAwards != null) {
            Debug.Log("[Timer] Starting end game award ceremony...");
            endGameAwards.ShowAwards();

            float duration = endGameAwards.displayDuration > 0 ? endGameAwards.displayDuration * 2f : 4f;
            yield return new WaitForSecondsRealtime(duration);
        }

        if (gameOverButtons != null) {
            gameOverButtons.SetActive(true);
            var buttonSprings = gameOverButtons.GetComponentsInChildren<SpringAPI>(true);
            foreach (var spring in buttonSprings) {
                spring.SetGoalValue(1f);
                spring.NudgeSpringVelocity();
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }
    private IEnumerator AnimateScoreCount(int finalScore, float duration) {
        float elapsed = 0f;
        int displayedScore = 0;

        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime; 
            float t = Mathf.Clamp01(elapsed / duration);
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, t));
            if (scoreAmountText != null)
                scoreAmountText.text = displayedScore.ToString("N0");
            yield return null;
        }

        if (scoreAmountText != null)
            scoreAmountText.text = finalScore.ToString("N0");
    }

    public void ReplayButton() {
        Time.timeScale = 1f;
        PointManager.Instance?.ResetPoints();
        PlayerManager.Instance?.ResetAllStats();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void MainMenuButton() {
        Time.timeScale = 1f;
        PointManager.Instance?.ResetPoints();
        PlayerManager.Instance?.ResetAllStats();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}
