using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class EndGameAwards : MonoBehaviour {
    [Header("Award Display")]
    [SerializeField] private GameObject awardDisplayPanel;
    [SerializeField] private Transform podiumParent; 
    [SerializeField] private TextMeshProUGUI awardTitleText;
    [SerializeField] private TextMeshProUGUI awardDescriptionText;
    [SerializeField] private TextMeshProUGUI playerNamesText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private float displayDuration = 4f;

    [Header("Camera")]
    [SerializeField] private Camera awardCamera;
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Podium Settings")]
    [SerializeField] private GameObject podiumPrefab;
    [SerializeField] private float podiumSpacing = 100f;
    [SerializeField] private float podiumMaxHeight = 3f;
    [SerializeField] private float podiumRiseSpeed = 2f;

    [Header("Spring Animations")]
    [SerializeField] private float springDelayBetweenTexts = 0.2f;

    private List<GameObject> currentPlayerModels = new List<GameObject>();
    private List<GameObject> podiums = new List<GameObject>();
    private bool ceremonyCancelled = false;

    public static EndGameAwards Instance { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        awardDisplayPanel.SetActive(false);
    }

    public void ShowAwards() {
        if (EndGameData.playerCount == 0 || EndGameData.playerObjects == null || EndGameData.playerObjects.Length == 0) {
            Debug.LogWarning("EndGameData is empty or missing player models. Cannot show awards.");
            return;
        }

        StartCoroutine(AwardCeremony());
    }

    public void CancelCeremony() {
        ceremonyCancelled = true;
        StopAllCoroutines();
        ClearPlayerModels();
        ClearPodiums();
        awardDisplayPanel.SetActive(false);
    }

    private IEnumerator AwardCeremony() {
        ceremonyCancelled = false;
        awardDisplayPanel.SetActive(true);

        yield return SetupPodiumsAndPlayers();

        yield return ShowAward("Hot Hands", "Most Ingredients Handled",
            GetMaxStatWinners(EndGameData.ingredientsHandled),
            i => $"Ingredients: {EndGameData.ingredientsHandled[i]}");
        if (ceremonyCancelled) yield break;

        yield return ShowAward("MVP", "Most Points Generated",
            GetMaxStatWinners(EndGameData.pointsGenerated),
            i => $"Points: {EndGameData.pointsGenerated[i]}");
        if (ceremonyCancelled) yield break;

        var guardianWinners = GetMaxStatWinners(EndGameData.jointsReconnected, requireNonZero: true);
        if (guardianWinners.Count > 0)
            yield return ShowAward("Guardian Angel", "Most Joints Reconnected",
                guardianWinners, i => $"Joints: {EndGameData.jointsReconnected[i]}");
        if (ceremonyCancelled) yield break;

        yield return ShowAward("Noob", "Worst Performance (Low Points + High Explosions)",
            GetNoobWinners(),
            i => $"Points: {EndGameData.pointsGenerated[i]} | Explosions: {EndGameData.explosionsReceived[i]}");

        yield return new WaitForSecondsRealtime(1f);
        awardDisplayPanel.SetActive(false);
    }

    private IEnumerator SetupPodiumsAndPlayers() {
        ClearPodiums();
        ClearPlayerModels();

        int count = EndGameData.playerCount;
        float maxPoints = Mathf.Max(1, Mathf.Max(EndGameData.pointsGenerated));

        float startX = -(count - 1) * podiumSpacing * 0.5f; // center the line

        for (int i = 0; i < count; i++) {
            Vector3 localPos = new Vector3(startX + i * podiumSpacing, 0, 0);
            GameObject podium = Instantiate(podiumPrefab, podiumParent);
            podium.transform.localPosition = localPos;
            podiums.Add(podium);

            float normalized = (float)EndGameData.pointsGenerated[i] / maxPoints;
            float targetHeight = Mathf.Lerp(0.5f, podiumMaxHeight, normalized);

            GameObject player = EndGameData.playerObjects[i];
            Transform playerTransform = null;

            if (player != null) {
                playerTransform = player.transform;
                currentPlayerModels.Add(player); // Add the reference original player
                StartCoroutine(RaisePodiumWithPlayer(podium.transform, playerTransform, targetHeight));
            } else {
                StartCoroutine(RaisePodiumWithPlayer(podium.transform, null, targetHeight));
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }

        yield return new WaitForSecondsRealtime(1.5f);
    }

    private IEnumerator RaisePodiumWithPlayer(Transform podium, Transform player, float targetHeight) {
        if (podium == null) yield break;

        Vector3 originalScale = podium.localScale;

        podium.localScale = new Vector3(originalScale.x, 0.01f, originalScale.z);

        float playerHeight = 1.5f;

        if (player != null) {
            player.gameObject.SetActive(true); 
            player.SetParent(podium, false);

            player.localPosition = Vector3.zero;
            player.localRotation = Quaternion.identity;
            player.localScale = Vector3.one;
     
            Bounds bounds = new Bounds(player.position, Vector3.zero);
            var renderers = player.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers) {
                if (r.enabled && r.gameObject.activeInHierarchy) {
                    bounds.Encapsulate(r.bounds);
                }
            }

            if (bounds.size.y > 0.1f) playerHeight = bounds.size.y;
        }

        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime * podiumRiseSpeed;
            float newYScale = Mathf.Lerp(0.01f, targetHeight, t);
            podium.localScale = new Vector3(originalScale.x, newYScale, originalScale.z);

            if (player != null) {

                player.localPosition = new Vector3(0, newYScale * 0.5f, 0);
            }

            yield return null;
        }
    }

    private IEnumerator ShowAward(string title, string description, List<int> winnerIndexes, Func<int, string> getStatText) {
        if (winnerIndexes == null || winnerIndexes.Count == 0) yield break;

        awardTitleText.text = title + (winnerIndexes.Count > 1 && title != "Hot Hands" ? "s" : "");
        awardDescriptionText.text = description;

        string playerNames = "", stats = "";
        for (int i = 0; i < winnerIndexes.Count; i++) {
            int idx = winnerIndexes[i];
            playerNames += $"Player {idx + 1}";
            stats += getStatText(idx);
            if (i < winnerIndexes.Count - 1) {
                playerNames += " & ";
                stats += " | ";
            }
        }
        playerNamesText.text = playerNames;
        statsText.text = stats;

        SpringAPI[] textSprings = {
            awardTitleText?.GetComponent<SpringAPI>(),
            awardDescriptionText?.GetComponent<SpringAPI>(),
            playerNamesText?.GetComponent<SpringAPI>(),
            statsText?.GetComponent<SpringAPI>()
        };

        foreach (var spring in textSprings) {
            if (spring != null) {
                spring.SetGoalValue(1f);
                spring.NudgeSpringVelocity();
            }
            yield return new WaitForSecondsRealtime(springDelayBetweenTexts);
        }

        float elapsed = 0f;
        Vector3 center = podiumParent.position;
        while (elapsed < displayDuration && !ceremonyCancelled) {
            elapsed += Time.unscaledDeltaTime;
            if (awardCamera != null) {
                awardCamera.transform.RotateAround(center, Vector3.up, rotationSpeed * Time.unscaledDeltaTime);
                awardCamera.transform.LookAt(center);
            }
            yield return null;
        }

        foreach (var spring in textSprings) {
            if (spring != null) {
                spring.SetGoalValue(0f);
                spring.NudgeSpringVelocity();
            }
            yield return new WaitForSecondsRealtime(springDelayBetweenTexts * 0.5f);
        }

        yield return new WaitForSecondsRealtime(0.5f);
    }

    private void ClearPodiums() {
        foreach (var podium in podiums)
            if (podium != null) Destroy(podium);
        podiums.Clear();
    }

    private void ClearPlayerModels() {
        Scene mainScene = SceneManager.GetSceneAt(0);

        foreach (var m in currentPlayerModels) {
            if (m != null && mainScene.IsValid()) {
                SceneManager.MoveGameObjectToScene(m, mainScene);
                m.SetActive(false);
            }
        }
        currentPlayerModels.Clear();
    }



    #region Stat Calculations
    private List<int> GetMaxStatWinners(int[] stats, bool requireNonZero = false) {
        int max = int.MinValue;
        for (int i = 0; i < EndGameData.playerCount; i++)
            if (stats[i] > max) max = stats[i];

        List<int> winners = new List<int>();
        for (int i = 0; i < EndGameData.playerCount; i++)
            if (stats[i] == max && (!requireNonZero || max > 0))
                winners.Add(i);

        return winners;
    }

    private List<int> GetNoobWinners() {
        int weight = 10;
        float worst = float.MaxValue;
        for (int i = 0; i < EndGameData.playerCount; i++) {
            float score = EndGameData.pointsGenerated[i] - EndGameData.explosionsReceived[i] * weight;
            if (score < worst) worst = score;
        }

        List<int> winners = new List<int>();
        for (int i = 0; i < EndGameData.playerCount; i++) {
            float score = EndGameData.pointsGenerated[i] - EndGameData.explosionsReceived[i] * weight;
            if (Mathf.Approximately(score, worst)) winners.Add(i);
        }
        return winners;
    }
    #endregion
}