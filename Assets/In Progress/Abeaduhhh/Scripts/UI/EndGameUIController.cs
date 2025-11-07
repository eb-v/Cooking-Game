using UnityEngine;
using System.Collections;
using TMPro;

public class EndGameUIController : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject starGroup;
    [SerializeField] private TextMeshProUGUI scoreAmountText;
    //[SerializeField] private GameObject gameOverButtons;

    [Header("Awards")]
    //[SerializeField] private EndGameAwards endGameAwards;
    [SerializeField] private float starPopDelay = 0.3f;

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
        int delivered = PointManager.Instance?.GetDeliveredCount() ?? 0;
        int finalScore = delivered * 300;
        int starsEarned = CalculateStars(finalScore);

        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        if (background != null) background.SetActive(true);

        if (scoreAmountText != null) {
            scoreAmountText.gameObject.SetActive(true);
            yield return StartCoroutine(AnimateScoreCount(finalScore, 1f));
        }

        yield return ShowStars(starsEarned);
        yield return new WaitForSecondsRealtime(1.5f);

        //if (endGameAwards != null)
        //    endGameAwards.ShowAwards();

        //if (gameOverButtons != null)
        //    gameOverButtons.SetActive(true);
    }

    private int CalculateStars(int score) {
        if (score >= 0) return 3;
        return 0;
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
