using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndGameAwardsSequencer : MonoBehaviour {
    [Header("Award Display")]
    [SerializeField] private GameObject awardDisplayPanel;
    [SerializeField] private Transform playerDisplayPosition;
    [SerializeField] private TextMeshProUGUI awardTitleText;
    [SerializeField] private TextMeshProUGUI awardDescriptionText;
    [SerializeField] private TextMeshProUGUI playerNamesText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private float displayDuration = 4f;

    [Header("Camera")]
    [SerializeField] private Camera awardCamera;
    [SerializeField] private float rotationSpeed = 30f;

    private List<GameObject> currentPlayerModels = new List<GameObject>();
    private bool ceremonyCancelled = false;

    public void ShowAwards() {
        Debug.Log("=== PRE-CEREMONY DEBUG ===");
        List<PlayerStats> debugPlayers = PlayerManager.Instance.GetAllPlayers();
        Debug.Log($"Total players found: {debugPlayers.Count}");
        foreach (var player in debugPlayers) {
            if (player != null) {
                Debug.Log($"Player {player.playerNumber} - Points: {player.pointsGenerated}, Ingredients: {player.ingredientsHandled}, Joints: {player.jointsReconnected}, Explosions: {player.explosionsReceived}");
            } else {
                Debug.LogWarning("Found NULL player in list!");
            }
        }
        Debug.Log("=== END PRE-CEREMONY DEBUG ===");

        StartCoroutine(AwardCeremony());
    }

    public void CancelCeremony() {
        ceremonyCancelled = true;
        StopAllCoroutines();
        ClearPlayerModels();
        awardDisplayPanel.SetActive(false);
    }

    private IEnumerator AwardCeremony() {
        ceremonyCancelled = false;
        awardDisplayPanel.SetActive(true);

        Debug.Log("Starting Award Ceremony - Looping Mode");

        while (!ceremonyCancelled) {
            yield return StartCoroutine(ShowAward(
                "Hot Hands",
                "Most Ingredients Handled",
                GetHotHandsWinners(),
                (player) => $"Ingredients: {player.ingredientsHandled}"
            ));

            if (ceremonyCancelled) yield break;

            yield return StartCoroutine(ShowAward(
                "MVP",
                "Most Points Generated",
                GetMVPWinners(),
                (player) => $"Points: {player.pointsGenerated}"
            ));

            if (ceremonyCancelled) yield break;

            var guardianWinners = GetGuardianAngelWinners();
            if (guardianWinners.Count > 0) {
                yield return StartCoroutine(ShowAward(
                    "Guardian Angel",
                    "Most Joints Reconnected",
                    guardianWinners,
                    (player) => $"Joints: {player.jointsReconnected}"
                ));
            }

            if (ceremonyCancelled) yield break;

            yield return StartCoroutine(ShowAward(
                "Noob",
                "Worst Performance (Low Points + High Explosions)",
                GetNoobWinners(),
                (player) => $"Points: {player.pointsGenerated} | Explosions: {player.explosionsReceived}"
            ));

            if (ceremonyCancelled) yield break;

            Debug.Log("Award Ceremony Loop Complete - Restarting...");
            yield return new WaitForSecondsRealtime(1f);
        }

        Debug.Log("Award Ceremony Ended");
    }

    private IEnumerator ShowAward(string title, string description, List<PlayerStats> winners, System.Func<PlayerStats, string> getStatText) {
        ClearPlayerModels();

        if (winners == null || winners.Count == 0) {
            Debug.Log($"Skipping award: {title} - No winners");
            yield break;
        }

        Debug.Log($"Showing award: {title}");

        if (awardDescriptionText != null)
            awardDescriptionText.text = description;

        string playerNames = "";
        string stats = "";
        for (int i = 0; i < winners.Count; i++) {
            playerNames += "Player " + winners[i].playerNumber;
            if (i < winners.Count - 1)
                playerNames += " & ";

            stats += getStatText(winners[i]);
            if (i < winners.Count - 1)
                stats += " | ";
        }

        string displayTitle = title;
        if (winners.Count > 1 && title != "Hot Hands")
            displayTitle += "s";

        if (playerNamesText != null)
            playerNamesText.text = playerNames;

        if (awardTitleText != null)
            awardTitleText.text = displayTitle;

        if (statsText != null)
            statsText.text = stats;

        float spacing = 3f;
        float startX = -(winners.Count - 1) * spacing / 2f;

        for (int i = 0; i < winners.Count; i++) {
            GameObject playerModel = CreateVisualOnlyCopy(
                winners[i].gameObject,
                playerDisplayPosition.position + new Vector3(startX + i * spacing, 0, 0),
                Quaternion.identity
            );

            playerModel.transform.SetParent(playerDisplayPosition);
            currentPlayerModels.Add(playerModel);
        }

        float elapsed = 0f;
        Vector3 centerPosition = playerDisplayPosition.position;

        while (elapsed < displayDuration && !ceremonyCancelled) {
            elapsed += Time.unscaledDeltaTime;

            if (awardCamera != null) {
                awardCamera.transform.RotateAround(
                    centerPosition,
                    Vector3.up,
                    rotationSpeed * Time.unscaledDeltaTime
                );
                awardCamera.transform.LookAt(centerPosition);
            }

            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);
    }

    private List<PlayerStats> GetHotHandsWinners() => GetWinners(p => p.ingredientsHandled);
    private List<PlayerStats> GetMVPWinners() => GetWinners(p => p.pointsGenerated, skipIfZero: true);
    private List<PlayerStats> GetGuardianAngelWinners() => GetWinners(p => p.jointsReconnected, skipIfZero: true);
    private List<PlayerStats> GetNoobWinners() {
        List<PlayerStats> allPlayers = PlayerManager.Instance.GetAllPlayers();
        if (allPlayers.Count == 0) return new List<PlayerStats>();

        float worstScore = float.MaxValue;
        int explosionWeight = 10;

        foreach (var player in allPlayers) {
            float noobScore = player.pointsGenerated - (player.explosionsReceived * explosionWeight);
            if (noobScore < worstScore) worstScore = noobScore;
        }

        return allPlayers.FindAll(p =>
            Mathf.Approximately(p.pointsGenerated - (p.explosionsReceived * explosionWeight), worstScore));
    }

    private List<PlayerStats> GetWinners(System.Func<PlayerStats, int> statSelector, bool skipIfZero = false) {
        List<PlayerStats> allPlayers = PlayerManager.Instance.GetAllPlayers();
        if (allPlayers.Count == 0) return new List<PlayerStats>();

        int maxValue = 0;
        foreach (var p in allPlayers)
            if (p != null && statSelector(p) > maxValue)
                maxValue = statSelector(p);

        if (skipIfZero && maxValue == 0)
            return new List<PlayerStats>();

        return allPlayers.FindAll(p => p != null && statSelector(p) == maxValue);
    }

    private GameObject CreateVisualOnlyCopy(GameObject original, Vector3 position, Quaternion rotation) {
        GameObject copy = new GameObject($"AwardDisplay_{original.name}");
        copy.transform.position = position;
        copy.transform.rotation = rotation;
        CopyVisualComponents(original.transform, copy.transform);
        return copy;
    }

    private void CopyVisualComponents(Transform source, Transform destination) {
        MeshFilter srcMF = source.GetComponent<MeshFilter>();
        if (srcMF) destination.gameObject.AddComponent<MeshFilter>().sharedMesh = srcMF.sharedMesh;

        MeshRenderer srcMR = source.GetComponent<MeshRenderer>();
        if (srcMR) destination.gameObject.AddComponent<MeshRenderer>().sharedMaterials = srcMR.sharedMaterials;

        SkinnedMeshRenderer srcSMR = source.GetComponent<SkinnedMeshRenderer>();
        if (srcSMR) {
            SkinnedMeshRenderer dest = destination.gameObject.AddComponent<SkinnedMeshRenderer>();
            dest.sharedMesh = srcSMR.sharedMesh;
            dest.sharedMaterials = srcSMR.sharedMaterials;
        }

        SpriteRenderer srcSR = source.GetComponent<SpriteRenderer>();
        if (srcSR) {
            SpriteRenderer dest = destination.gameObject.AddComponent<SpriteRenderer>();
            dest.sprite = srcSR.sprite;
            dest.material = srcSR.material;
        }

        foreach (Transform child in source) {
            GameObject childCopy = new GameObject(child.name);
            childCopy.transform.SetParent(destination);
            childCopy.transform.localPosition = child.localPosition;
            childCopy.transform.localRotation = child.localRotation;
            childCopy.transform.localScale = child.localScale;
            CopyVisualComponents(child, childCopy.transform);
        }
    }

    private void ClearPlayerModels() {
        foreach (var model in currentPlayerModels)
            if (model != null)
                Destroy(model);
        currentPlayerModels.Clear();
    }
}
