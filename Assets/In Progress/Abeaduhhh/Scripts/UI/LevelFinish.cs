//using UnityEngine;
//using System.Collections;
//using TMPro;

//public class LevelFinishSequencer : MonoBehaviour {
//    [Header("Spring References")]
//    public SpringAPI levelCompleteSpring;
//    public SpringAPI star1Spring;
//    public SpringAPI star2Spring;
//    public SpringAPI star3Spring;

//    [Header("Star GameObjects")]
//    public GameObject star1GameObject;
//    public GameObject star2GameObject;
//    public GameObject star3GameObject;

//    [Header("Score UI")]
//    public GameObject scoreGameObject;
//    public TextMeshProUGUI scoreText;

//    [Header("Sequence Timing")]
//    public float delayBeforeStars = 0.5f;
//    public float delayBetweenStars = 0.3f;
//    public float delayBeforeScore = 0.5f;
//    public float delayAfterScore = 1.0f;
//    public float delayBetweenSpringOut = 0.25f;
//    public float scoreCountDuration = 1.0f;

//    [Header("Testing (Optional)")]
//    public bool testOnStart = true;
//    public int testStars = 2;
//    public int testScore = 12500;

//    private int _finalScore;

//    private void Start() {
//        if (scoreText != null) scoreText.text = "";

//        if (testOnStart) {
//            ResetAllSprings();
//            StartSequence(testStars, testScore);
//        }
//    }

//    /// <summary>
//    /// Starts the animation sequence after level completion.
//    /// </summary>
//    public void StartSequence(int starsEarned, int finalScore) {
//        _finalScore = finalScore;

//        // Hide stars & score initially
//        star1GameObject.SetActive(false);
//        star2GameObject.SetActive(false);
//        star3GameObject.SetActive(false);
//        if (scoreGameObject != null) scoreGameObject.SetActive(false);

//        StartCoroutine(RunSequence(starsEarned));
//    }

//    private IEnumerator RunSequence(int starsEarned) {
//        // --- PHASE 1: LEVEL COMPLETE SPRING IN ---
//        levelCompleteSpring.SetGoalValue(1f);
//        levelCompleteSpring.NudgeSpringVelocity();

//        yield return new WaitForSeconds(delayBeforeStars);

//        // --- STARS SPRING IN ONE BY ONE ---
//        if (starsEarned >= 1) {
//            star1GameObject.SetActive(true);
//            star1Spring.SetGoalValue(1f);
//            star1Spring.NudgeSpringVelocity();
//            yield return new WaitForSeconds(delayBetweenStars);
//        }

//        if (starsEarned >= 2) {
//            star2GameObject.SetActive(true);
//            star2Spring.SetGoalValue(1f);
//            star2Spring.NudgeSpringVelocity();
//            yield return new WaitForSeconds(delayBetweenStars);
//        }

//        if (starsEarned >= 3) {
//            star3GameObject.SetActive(true);
//            star3Spring.SetGoalValue(1f);
//            star3Spring.NudgeSpringVelocity();
//        }

//        // --- SHOW SCORE ---
//        yield return new WaitForSeconds(delayBeforeScore);
//        if (scoreGameObject != null) scoreGameObject.SetActive(true);

//        if (scoreText != null) {
//            yield return StartCoroutine(AnimateScoreCount(_finalScore, scoreCountDuration));
//        }

//        // --- WAIT BEFORE SPRING OUT ---
//        yield return new WaitForSeconds(delayAfterScore);

//        // --- PHASE 2: SPRING OUT (reverse order) ---
//        // STARS OUT (reverse order)
//        if (starsEarned >= 3) {
//            star3Spring.SetGoalValue(0f);
//            star3Spring.NudgeSpringVelocity();
//            yield return new WaitForSeconds(delayBetweenSpringOut);
//            star3GameObject.SetActive(false);
//        }

//        if (starsEarned >= 2) {
//            star2Spring.SetGoalValue(0f);
//            star2Spring.NudgeSpringVelocity();
//            yield return new WaitForSeconds(delayBetweenSpringOut);
//            star2GameObject.SetActive(false);
//        }

//        if (starsEarned >= 1) {
//            star1Spring.SetGoalValue(0f);
//            star1Spring.NudgeSpringVelocity();
//            yield return new WaitForSeconds(delayBetweenSpringOut);
//            star1GameObject.SetActive(false);
//        }

//        if (scoreGameObject != null) scoreGameObject.SetActive(false);
//        if (scoreText != null) scoreText.text = "";

//        // LEVEL COMPLETE OUT LAST
//        levelCompleteSpring.SetGoalValue(0f);
//        levelCompleteSpring.NudgeSpringVelocity();
//        yield return new WaitForSeconds(delayBetweenSpringOut);
//        levelCompleteSpring.gameObject.SetActive(false);


//    }

//    private IEnumerator AnimateScoreCount(int finalScore, float duration) {
//        float elapsed = 0f;
//        int displayedScore = 0;

//        while (elapsed < duration) {
//            elapsed += Time.unscaledDeltaTime; 
//            float t = Mathf.Clamp01(elapsed / duration);
//            displayedScore = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, t));
//            scoreText.text = displayedScore.ToString("N0");
//            yield return null;
//        }

//        scoreText.text = finalScore.ToString("N0");
//    }
//    public void ResetAllSprings() {
//        levelCompleteSpring.ResetSpring();
//        star1Spring.ResetSpring();
//        star2Spring.ResetSpring();
//        star3Spring.ResetSpring();

//        star1GameObject.SetActive(false);
//        star2GameObject.SetActive(false);
//        star3GameObject.SetActive(false);
//        if (scoreGameObject != null) scoreGameObject.SetActive(false);
//    }
//}
