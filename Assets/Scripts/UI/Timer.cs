using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {
    [Header("Timer Settings")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float startTime = 300f;

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
    //private GameObject CreateVisualOnlyCopy(GameObject original, Vector3 position, Quaternion rotation) {
    //    GameObject copy = new GameObject(original.name + "_Visual");
    //    copy.transform.position = position;
    //    copy.transform.rotation = rotation;

    //    // Copy MeshRenderers
    //    foreach (var renderer in original.GetComponentsInChildren<MeshRenderer>(true)) {
    //        var go = new GameObject(renderer.gameObject.name);
    //        go.transform.SetParent(copy.transform);
    //        go.transform.localPosition = renderer.transform.localPosition;
    //        go.transform.localRotation = renderer.transform.localRotation;

    //        var meshFilter = go.AddComponent<MeshFilter>();
    //        meshFilter.sharedMesh = renderer.GetComponent<MeshFilter>()?.sharedMesh;

    //        var meshRenderer = go.AddComponent<MeshRenderer>();
    //        meshRenderer.sharedMaterials = renderer.sharedMaterials;
    //    }

    //    // Copy SkinnedMeshRenderers
    //    foreach (var skinned in original.GetComponentsInChildren<SkinnedMeshRenderer>(true)) {
    //        var go = new GameObject(skinned.gameObject.name);
    //        go.transform.SetParent(copy.transform);
    //        go.transform.localPosition = skinned.transform.localPosition;
    //        go.transform.localRotation = skinned.transform.localRotation;

    //        var skinnedCopy = go.AddComponent<SkinnedMeshRenderer>();
    //        skinnedCopy.sharedMesh = skinned.sharedMesh;
    //        skinnedCopy.sharedMaterials = skinned.sharedMaterials;
    //    }

    //    return copy;
    //}

    //private IEnumerator HandleGameOver() {
    //    int delivered = PointManager.Instance != null ? PointManager.Instance.GetDeliveredCount() : 0;
    //    int finalScore = delivered * 300;
    //    int starsEarned = CalculateStars(finalScore);

    //    // --- STEP 1: Show game over canvas ---
    //    if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
    //    if (background != null) background.SetActive(true);

    //    // Animate score counting up
    //    if (scoreAmountText != null) {
    //        scoreAmountText.gameObject.SetActive(true);
    //        yield return StartCoroutine(AnimateScoreCount(finalScore, 1.0f));
    //    }

    //    yield return ShowStars(starsEarned);

    //    yield return new WaitForSecondsRealtime(1.5f);

    //    //  Reverse GameOverCanvas and Stars
    //    if (gameOverCanvas != null) {
    //        foreach (Transform child in gameOverCanvas.transform) {
    //            var springs = child.GetComponentsInChildren<SpringAPI>(true);
    //            foreach (var spring in springs) {
    //                spring.SetGoalValue(0f);
    //                spring.NudgeSpringVelocity();
    //            }
    //        }
    //    }

    //    if (starGroup != null) {
    //        for (int i = 0; i < starGroup.transform.childCount; i++) {
    //            var star = starGroup.transform.GetChild(i).gameObject;
    //            var spring = star.GetComponent<SpringAPI>();
    //            spring?.SetGoalValue(0f);
    //            spring?.NudgeSpringVelocity();
    //        }
    //    }

    //    yield return new WaitForSecondsRealtime(0.75f);

    //    if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
    //    if (starGroup != null) starGroup.SetActive(false);

    //    if (endGameCanvas != null) {
    //        endGameCanvas.SetActive(true);

    //        yield return new WaitForSecondsRealtime(0.3f);
    //    }

    //    if (endGameAwards != null) {
    //        Debug.Log("[Timer] Starting end game award ceremony...");
    //        endGameAwards.ShowAwards();

    //        float duration = endGameAwards.displayDuration > 0 ? endGameAwards.displayDuration * 2f : 4f;
    //        yield return new WaitForSecondsRealtime(duration);
    //    }

    //    if (gameOverButtons != null) {
    //        gameOverButtons.SetActive(true);
    //        var buttonSprings = gameOverButtons.GetComponentsInChildren<SpringAPI>(true);
    //        foreach (var spring in buttonSprings) {
    //            spring.SetGoalValue(1f);
    //            spring.NudgeSpringVelocity();
    //            yield return new WaitForSecondsRealtime(0.1f);
    //        }
    //    }
    //}
    private IEnumerator HandleGameOver() {
        //UI animations
        int delivered = PointManager.Instance != null ? PointManager.Instance.GetDeliveredCount() : 0;
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

        if (gameOverCanvas != null) {
            foreach (Transform child in gameOverCanvas.transform) {
                foreach (var spring in child.GetComponentsInChildren<SpringAPI>(true)) {
                    spring.SetGoalValue(0f);
                    spring.NudgeSpringVelocity();
                }
            }
        }

        if (starGroup != null) {
            foreach (Transform star in starGroup.transform) {
                star.GetComponent<SpringAPI>()?.SetGoalValue(0f);
            }
        }

        yield return new WaitForSecondsRealtime(0.75f);

        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
        if (starGroup != null) starGroup.SetActive(false);

        // Store ORIGINAL player reference and stats for End Game Scene
        if (PlayerManager.Instance == null) {
            Debug.LogError("[Timer] PlayerManager.Instance is null! Cannot save end game data.");
            yield break;
        }

        List<PlayerStats> players = PlayerManager.Instance.GetAllPlayers();
        if (players == null || players.Count == 0) {
            Debug.LogWarning("[Timer] No players found to save for end game.");
            yield break;
        }

        Debug.Log($"[Timer] Preparing {players.Count} players for AwardScene.");

        int playerCount = players.Count;
        EndGameData.playerCount = playerCount;
        EndGameData.playerObjects = new GameObject[playerCount]; 
        EndGameData.pointsGenerated = new int[playerCount];
        EndGameData.ingredientsHandled = new int[playerCount];
        EndGameData.jointsReconnected = new int[playerCount];
        EndGameData.explosionsReceived = new int[playerCount];

        var playerGameObjects = PlayerManager.Instance.players;

        for (int i = 0; i < playerCount; i++) {
            var stats = players[i];
            if (stats == null) {
                Debug.LogWarning($"[Timer] Player {i} stats are null!");
                continue;
            }

            if (i < playerGameObjects.Count && playerGameObjects[i] != null) {
                GameObject original = playerGameObjects[i];

                foreach (var rb in original.GetComponentsInChildren<Rigidbody>()) rb.isKinematic = true;
                foreach (var c in original.GetComponentsInChildren<Collider>()) c.enabled = false;

                // 2. Hide the original player's mesh/model temporarily
                original.SetActive(false);

                DontDestroyOnLoad(original);

                // 4. Store the original object reference
                EndGameData.playerObjects[i] = original;

                Debug.Log($"[Timer] Saved ORIGINAL Player {i} ({original.name}).");

            } else {
                EndGameData.playerObjects[i] = null;
                Debug.LogWarning($"[Timer] Player GameObject at index {i} is missing!");
            }

            // Store stats
            EndGameData.pointsGenerated[i] = stats.pointsGenerated;
            EndGameData.ingredientsHandled[i] = stats.ingredientsHandled;
            EndGameData.jointsReconnected[i] = stats.jointsReconnected;
            EndGameData.explosionsReceived[i] = stats.explosionsReceived;
        }

        // End Game Scene Additively ---
        Debug.Log("[Timer] Loading AwardScene 1 additively...");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("AwardScene 1", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
            yield return null;

        Scene awardScene = SceneManager.GetSceneByName("AwardScene 1");
        if (!awardScene.IsValid()) {
            Debug.LogError("[Timer] AwardScene 1 failed to load.");
            yield break;
        }

        Debug.Log("[Timer] Successfully loaded AwardScene 1.");

        // MOVING THE ORIGINAL PLAYER OBJECT TO THE AWARD SCENE ******************
        for (int i = 0; i < EndGameData.playerObjects.Length; i++) {
            GameObject originalPlayer = EndGameData.playerObjects[i];
            if (originalPlayer == null) continue;

            // MOVING THE ORIGINAL PLAYER OBJECT TO THE AWARD SCENE ******************
            SceneManager.MoveGameObjectToScene(originalPlayer, awardScene);
            Debug.Log($"[Timer] Moved ORIGINAL Player {i} ({originalPlayer.name}) to AwardScene.");
        }

        //Trigger award ceremony 
        while (EndGameAwards.Instance == null)
            yield return null;

        if (EndGameAwards.Instance != null) {
            EndGameAwards.Instance.gameObject.SetActive(true);
            EndGameAwards.Instance.ShowAwards();
        } else {
            Debug.LogError("[Timer] EndGameAwards.Instance is null!");
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
