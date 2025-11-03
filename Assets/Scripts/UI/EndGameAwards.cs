using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameAwards : MonoBehaviour {
    [Header("Award Display")]
    [SerializeField] private GameObject awardDisplayPanel;
    [SerializeField] private Transform podiumParent; // Parent for platforms and players
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

        // Step 1: Setup podiums and players
        yield return SetupPodiumsAndPlayers();

        // Step 2: Run awards
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
            // Spawn podium
            Vector3 localPos = new Vector3(startX + i * podiumSpacing, 0, 0);
            GameObject podium = Instantiate(podiumPrefab, podiumParent);
            podium.transform.localPosition = localPos;
            podiums.Add(podium);

            float normalized = (float)EndGameData.pointsGenerated[i] / maxPoints;
            float targetHeight = Mathf.Lerp(0.5f, podiumMaxHeight, normalized);

            // Spawn visual player
            GameObject playerCopy = null;
            GameObject player = EndGameData.playerObjects[i];
            if (player != null) {
                playerCopy = CreateVisualOnlyCopy(player, podium.transform.position, Quaternion.identity);
                currentPlayerModels.Add(playerCopy);
                StartCoroutine(RaisePodiumWithPlayer(podium.transform, playerCopy.transform, targetHeight));
            } else {
                StartCoroutine(RaisePodiumWithPlayer(podium.transform, null, targetHeight));
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }

        yield return new WaitForSecondsRealtime(1.5f);
    }

    private IEnumerator RaisePodiumWithPlayer(Transform podium, Transform player, float targetHeight) {
        Vector3 originalScale = podium.localScale;
        float t = 0f;
        podium.localScale = new Vector3(originalScale.x, 0.1f, originalScale.z);

        float playerHeight = 1f;
        if (player != null) {
            Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0) {
                Bounds bounds = renderers[0].bounds;
                foreach (var r in renderers) bounds.Encapsulate(r.bounds);
                playerHeight = bounds.size.y;
            }
        }

        while (t < 1f) {
            t += Time.unscaledDeltaTime * podiumRiseSpeed;
            float newYScale = Mathf.Lerp(0.1f, targetHeight, t);
            podium.localScale = new Vector3(originalScale.x, newYScale, originalScale.z);

            if (player != null) {
                Vector3 podiumTop = podium.position + Vector3.up * newYScale;
                player.position = podiumTop + Vector3.up * (playerHeight * 0.5f);
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
        foreach (var m in currentPlayerModels)
            if (m != null) Destroy(m);
        currentPlayerModels.Clear();
    }

    private GameObject CreateVisualOnlyCopy(GameObject original, Vector3 position, Quaternion rotation) {
        GameObject copy = new GameObject(original.name + "_Visual");
        copy.transform.position = position;
        copy.transform.rotation = rotation;
        copy.transform.localScale = original.transform.lossyScale;

        // Copy all MeshRenderers
        foreach (var meshRenderer in original.GetComponentsInChildren<MeshRenderer>(true)) {
            var mf = meshRenderer.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;

            GameObject go = new GameObject(meshRenderer.gameObject.name);
            go.transform.SetParent(copy.transform, false);
            go.transform.localPosition = meshRenderer.transform.localPosition;
            go.transform.localRotation = meshRenderer.transform.localRotation;
            go.transform.localScale = meshRenderer.transform.localScale;

            var newMF = go.AddComponent<MeshFilter>();
            newMF.sharedMesh = mf.sharedMesh;

            var newMR = go.AddComponent<MeshRenderer>();
            // Clone materials so they render independently
            newMR.materials = meshRenderer.sharedMaterials;
            newMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            newMR.receiveShadows = true;
        }

        // Copy SkinnedMeshRenderers
        foreach (var skinned in original.GetComponentsInChildren<SkinnedMeshRenderer>(true)) {
            if (skinned.sharedMesh == null) continue;

            GameObject go = new GameObject(skinned.gameObject.name);
            go.transform.SetParent(copy.transform, false);
            go.transform.localPosition = skinned.transform.localPosition;
            go.transform.localRotation = skinned.transform.localRotation;
            go.transform.localScale = skinned.transform.localScale;

            var newSkinned = go.AddComponent<SkinnedMeshRenderer>();
            newSkinned.sharedMesh = skinned.sharedMesh;
            newSkinned.materials = skinned.sharedMaterials;
            newSkinned.bones = skinned.bones;
            newSkinned.rootBone = skinned.rootBone;
        }

        return copy;
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
