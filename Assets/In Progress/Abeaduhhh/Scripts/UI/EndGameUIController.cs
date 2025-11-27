using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameUIController : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject awardsCanvas;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject starGroup;
    [SerializeField] private TextMeshProUGUI scoreAmountText;
    //[SerializeField] private GameObject gameOverButtons;
    [SerializeField] private SceneField _playScene;
    private readonly List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();


    [Header("Awards")]
    [SerializeField] private float starPopDelay = 0.3f;
    [SerializeField] private float starWaitAfterPop = 1.5f;
    //[SerializeField] private float starPopOutDelay = 0.2f;
    [SerializeField] private float gameOverPopOutDelay = 0.75f;

    //[SerializeField] private EndGameAwards endGameAwards;

    private void Awake() {
        GenericEvent<ShowEndGameUIEvent>.GetEvent("EndGameUI").AddListener(OnShowEndGameUI);
        GenericEvent<HideEndGameUIEvent>.GetEvent("EndGameUI").AddListener(OnHideEndGameUI);
    }

    private void OnDestroy() {
        GenericEvent<ShowEndGameUIEvent>.GetEvent("EndGameUI").RemoveListener(OnShowEndGameUI);
        GenericEvent<HideEndGameUIEvent>.GetEvent("EndGameUI").RemoveListener(OnHideEndGameUI);
    }

    private void OnShowEndGameUI() {
        Debug.Log("[EndGameUIController] ShowEndGameUIEvent received!");
        StartCoroutine(ShowEndGameSequence());
    }

    private void OnHideEndGameUI() {
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);

        if (background != null)
            background.SetActive(false);
    }

    private IEnumerator ShowEndGameSequence() {
        int finalScore = ScoreSystem.Instance.GetCurrentScore();
        int starsEarned = CalculateStars(finalScore);

        if (background != null) background.SetActive(true);
        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);

        if (scoreAmountText != null) {
            scoreAmountText.gameObject.SetActive(true);
            yield return StartCoroutine(AnimateScoreCount(finalScore, 1f));
        }

        yield return ShowStars(starsEarned);

        yield return new WaitForSecondsRealtime(starWaitAfterPop);

        yield return HideStars(starsEarned);

        yield return SpringOutGameOverCanvas();

        //if (endGameAwards != null) {
        //    yield return endGameAwards.ShowAwards(this); // Wait until the ceremony finishes
        //}

        //if (gameOverButtons != null)
        //    gameOverButtons.SetActive(true);

        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_playScene));

    }

    private int CalculateStars(int score) {
        //if (score >= 0) return 3;
            if (score >= 2500) return 3;
            if (score >= 2000) return 2;
            if (score >= 1500) return 1;
        return 0;
    }
    private IEnumerator SpringOutGameOverCanvas() {
        if (gameOverCanvas == null) yield break;

        // Get all active SpringAPI components in the GameOver UI hierarchy
        SpringAPI[] springs = gameOverCanvas.GetComponentsInChildren<SpringAPI>(true);

        if (springs.Length > 0) {
            Debug.Log("[EndGameUIController] Springing OUT GameOverCanvas contents...");

            // Tell each one to spring back (toward minGoal)
            foreach (var spring in springs) {
                spring.HideSpring();  // move back toward original (minGoal)
                yield return new WaitForSecondsRealtime(0.05f); // small stagger
            }

            // Wait for the springs to settle before hiding everything
            yield return new WaitForSecondsRealtime(gameOverPopOutDelay);
        }

        // Finally hide the canvases
        gameOverCanvas.SetActive(false);
        if (background != null)
            background.SetActive(false);

        

    }

    private IEnumerator ShowStars(int starsEarned) {
        if (starGroup == null) yield break;

        starGroup.SetActive(true);

        for (int i = 0; i < starGroup.transform.childCount; i++)
            starGroup.transform.GetChild(i).gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(0.75f);

        for (int i = 0; i < starsEarned && i < starGroup.transform.childCount; i++) {
            var star = starGroup.transform.GetChild(i).gameObject;
            star.SetActive(true);
            star.GetComponent<SpringAPI>()?.PlaySpring();
            yield return new WaitForSecondsRealtime(starPopDelay);
        }
    }

    private IEnumerator HideStars(int starsEarned) {
        if (starGroup == null) yield break;

        // Reverse order for a cascading feel
        for (int i = starsEarned - 1; i >= 0; i--) {
            if (i < starGroup.transform.childCount) {
                var star = starGroup.transform.GetChild(i).gameObject;
                var spring = star.GetComponent<SpringAPI>();

                if (spring != null) {
                    // Make it return smoothly to minGoal (e.g. scale 0)
                    spring.HideSpring();
                }

                // Smooth fade (optional, requires CanvasGroup or Image)
                var canvasGroup = star.GetComponent<CanvasGroup>();
                if (canvasGroup != null) {
                    StartCoroutine(FadeOut(canvasGroup, 0.25f));
                }

                yield return new WaitForSecondsRealtime(0.15f); // slight overlap for smooth cascading
            }
        }

        // Wait for the last star’s animation to finish
        yield return new WaitForSecondsRealtime(0.4f);

        starGroup.SetActive(false);
    }
    private IEnumerator FadeOut(CanvasGroup cg, float duration) {
        float elapsed = 0f;
        float startAlpha = cg.alpha;

        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        cg.alpha = 0f;
    }

    private IEnumerator AnimateScoreCount(int finalScore, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int score = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, t));
            if (scoreAmountText != null)
                scoreAmountText.text = score.ToString("N0");
            yield return null;
        }

        if (scoreAmountText != null)
            scoreAmountText.text = finalScore.ToString("N0");
    }
}
