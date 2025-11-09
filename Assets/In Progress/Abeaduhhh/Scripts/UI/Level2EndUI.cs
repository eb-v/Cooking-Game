//using UnityEngine;
//using TMPro;
//using System.Collections;

//public class Level2EndUI : EndGameUIOverride {
//    [Header("UI References")]
//    [SerializeField] private GameObject gameOverCanvas;
//    [SerializeField] private GameObject background;
//    [SerializeField] private GameObject starGroup;
//    [SerializeField] private TextMeshProUGUI scoreText;

//    [Header("Settings")]
//    [SerializeField] private float starPopDelay = 0.3f;

//    private void Awake() {
//        GenericEvent<ShowEndGameUIEvent>.GetEvent("EndGameUI").AddListener(OnShowEndGameUIEvent);
//    }

//    private void OnDestroy() {
//        GenericEvent<ShowEndGameUIEvent>.GetEvent("EndGameUI").RemoveListener(OnShowEndGameUIEvent);
//    }

//    private void OnShowEndGameUIEvent() {
//        ShowEndGameUI();
//    }

//    public override void ShowEndGameUI() {
//        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
//        if (background != null) background.SetActive(true);

//        int finalScore = 1000;
//        StartCoroutine(ShowEndGameSequence(finalScore, starsEarned));
//    }

//    private IEnumerator ShowEndGameSequence(int finalScore, int starsEarned) {
//        if (scoreText != null) {
//            scoreText.gameObject.SetActive(true);
//            yield return StartCoroutine(AnimateScore(finalScore));
//        }

//        yield return StartCoroutine(ShowStars(starsEarned));
//        yield return new WaitForSecondsRealtime(1.5f);
//    }

//    public override IEnumerator AnimateScore(int finalScore) {
//        float duration = 1f;
//        float elapsed = 0f;

//        while (elapsed < duration) {
//            elapsed += Time.unscaledDeltaTime;
//            float t = Mathf.Clamp01(elapsed / duration);
//            int score = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, t));
//            if (scoreText != null)
//                scoreText.text = score.ToString("N0");
//            yield return null;
//        }

//        if (scoreText != null)
//            scoreText.text = finalScore.ToString("N0");
//    }

//    public override IEnumerator ShowStars(int starsEarned) {
//        if (starGroup == null) yield break;

//        starGroup.SetActive(true);
//        for (int i = 0; i < starGroup.transform.childCount; i++)
//            starGroup.transform.GetChild(i).gameObject.SetActive(false);

//        yield return new WaitForSecondsRealtime(0.75f);

//        for (int i = 0; i < starsEarned && i < starGroup.transform.childCount; i++) {
//            var star = starGroup.transform.GetChild(i).gameObject;
//            star.SetActive(true);
//            star.GetComponent<SpringAPI>()?.PlaySpring();
//            yield return new WaitForSecondsRealtime(starPopDelay);
//        }
//    }
//}
