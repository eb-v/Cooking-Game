using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameAwards : MonoBehaviour {
    [Header("Award Display")]
    [SerializeField] private GameObject awardDisplayPanel;
    //[SerializeField] private Transform podiumParent;
    [SerializeField] private TextMeshProUGUI awardTitleText;
    [SerializeField] private TextMeshProUGUI awardDescriptionText;
    [SerializeField] private TextMeshProUGUI playerNamesText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private float displayDuration = 4f;
    [SerializeField] private GameOverButtonsController buttonsController;

    [Header("Camera")]
    [SerializeField] private Camera awardCamera;
    [SerializeField] private float rotationSpeed = 30f;

    //[Header("Podium Settings")]
    //[SerializeField] private GameObject podiumPrefab;
    //[SerializeField] private float podiumSpacing = 100f;
    //[SerializeField] private float podiumMaxHeight = 3f;
    //[SerializeField] private float podiumRiseSpeed = 2f;

    [Header("Spring Animations")]
    [SerializeField] private float springDelayBetweenTexts = 0f;


    private List<PlayerStats> players = new List<PlayerStats>();
    //private List<GameObject> podiums = new List<GameObject>();
    private bool ceremonyCancelled = false;

    public static EndGameAwards Instance { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        awardDisplayPanel.SetActive(false);
    }

    public Coroutine ShowAwards(MonoBehaviour caller) {

        if (EndGameData.playerCount == 0) {
            Debug.LogWarning("No players found. Cannot show awards.");
            return null;
        }

        // Build a temporary list of PlayerStats using the static data
        players = new List<PlayerStats>();
        for (int i = 0; i < EndGameData.playerCount; i++) {
            PlayerStats p = EndGameData.playerObjects[i].GetComponent<PlayerStats>();
            players.Add(p);
        }
        for (int i = 0; i < EndGameData.playerCount; i++) {
            PlayerStats temp = new PlayerStats();
            temp.playerNumber = i + 1;
            temp.ingredientsHandled = EndGameData.ingredientsHandled[i];
            temp.pointsGenerated = EndGameData.pointsGenerated[i];
            temp.jointsReconnected = EndGameData.jointsReconnected[i];
            temp.explosionsReceived = EndGameData.explosionsReceived[i];
            players.Add(temp);
        }

        return caller.StartCoroutine(AwardCeremony());
    }

    public void CancelCeremony() {
        ceremonyCancelled = true;
        StopAllCoroutines();
        awardDisplayPanel.SetActive(false);
    }

    private IEnumerator AwardCeremony() {
        ceremonyCancelled = false;
        awardDisplayPanel.SetActive(true);

        //yield return SetupPodiumsAndPlayers();

        // Awards
        yield return ShowAward("Hot Hands", "Most Ingredients Handled",
            GetMaxStatWinners(p => p.ingredientsHandled),
            p => $"Ingredients: {p.ingredientsHandled}");
        if (ceremonyCancelled) yield break;

        yield return ShowAward("MVP", "Most Points Generated",
            GetMaxStatWinners(p => p.pointsGenerated),
            p => $"Points: {p.pointsGenerated}");
        if (ceremonyCancelled) yield break;

        var guardianWinners = GetMaxStatWinners(p => p.jointsReconnected, requireNonZero: true);
        if (guardianWinners.Count > 0)
            yield return ShowAward("Guardian Angel", "Most Joints Reconnected",
                guardianWinners, p => $"Joints: {p.jointsReconnected}");

        buttonsController?.ShowButtons();

        if (ceremonyCancelled) yield break;

        yield return ShowAward("Noob", "Worst Performance (Low Points + High Explosions)",
            GetNoobWinners(),
            p => $"Points: {p.pointsGenerated} | Explosions: {p.explosionsReceived}");

        yield return new WaitForSecondsRealtime(1f);
        awardDisplayPanel.SetActive(false);
    }

    //private IEnumerator SetupPodiumsAndPlayers() {
    //    ClearPodiums();

    //    int count = players.Count;
    //    float maxPoints = Mathf.Max(1, players.Max(p => p.pointsGenerated));

    //    float startX = -(count - 1) * podiumSpacing * 0.5f;

    //    for (int i = 0; i < count; i++) {
    //        var p = players[i];
    //        Vector3 localPos = new Vector3(startX + i * podiumSpacing, 0, 0);

    //        GameObject podium = Instantiate(podiumPrefab, podiumParent);
    //        podium.transform.localPosition = localPos;
    //        podiums.Add(podium);

    //        float normalized = (float)p.pointsGenerated / maxPoints;
    //        float targetHeight = Mathf.Lerp(0.5f, podiumMaxHeight, normalized);

    //        GameObject playerObj = p.GetPlayerPrefab();
    //        StartCoroutine(RaisePodiumWithPlayer(podium.transform, playerObj.transform, targetHeight));

    //        yield return new WaitForSecondsRealtime(0.1f);
    //    }

    //    yield return new WaitForSecondsRealtime(1.5f);
    //}

    //private IEnumerator RaisePodiumWithPlayer(Transform podium, Transform player, float targetHeight) {
    //    if (podium == null) yield break;

    //    Vector3 originalScale = podium.localScale;
    //    podium.localScale = new Vector3(originalScale.x, 0.01f, originalScale.z);

    //    player.SetParent(podium, false);
    //    player.localPosition = Vector3.zero;
    //    player.localRotation = Quaternion.identity;
    //    player.localScale = Vector3.one;

    //    float t = 0f;
    //    while (t < 1f) {
    //        t += Time.unscaledDeltaTime * podiumRiseSpeed;
    //        float newYScale = Mathf.Lerp(0.01f, targetHeight, t);
    //        podium.localScale = new Vector3(originalScale.x, newYScale, originalScale.z);
    //        player.localPosition = new Vector3(0, newYScale * 0.5f, 0);
    //        yield return null;
    //    }
    //}

    private IEnumerator ShowAward(
      string title,
      string description,
      List<PlayerStats> winners,
      System.Func<PlayerStats, string> getStatText) {
        if (winners == null || winners.Count == 0) yield break;

        awardTitleText.text = title + (winners.Count > 1 && title != "Hot Hands" ? "s" : "");
        awardDescriptionText.text = description;
        playerNamesText.text = string.Join(" & ", winners.Select(p => $"Player {p.playerNumber}"));
        statsText.text = string.Join(" | ", winners.Select(getStatText));

        SpringAPI[] springs =
        {
        awardTitleText?.GetComponent<SpringAPI>(),
        awardDescriptionText?.GetComponent<SpringAPI>(),
        playerNamesText?.GetComponent<SpringAPI>(),
        statsText?.GetComponent<SpringAPI>()
    };

        foreach (var spring in springs) {
            if (spring != null)
                spring.PlaySpring();

            yield return new WaitForSecondsRealtime(springDelayBetweenTexts);
        }

        float elapsed = 0f;
        while (elapsed < displayDuration && !ceremonyCancelled) {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        foreach (var spring in springs) {
            if (spring != null) {
                spring.HideSpring();
                spring.NudgeSpringVelocity();
            }

            yield return new WaitForSecondsRealtime(springDelayBetweenTexts * 0f);
        }

        yield return new WaitForSecondsRealtime(0.5f);
    }
    #region Stat Calculations
    private List<PlayerStats> GetMaxStatWinners(System.Func<PlayerStats, int> selector, bool requireNonZero = false) {
        int max = players.Max(selector);
        if (requireNonZero && max <= 0) return new List<PlayerStats>();
        return players.Where(p => selector(p) == max).ToList();
    }

    private List<PlayerStats> GetNoobWinners() {
        int weight = 10;
        float worst = players.Min(p => p.pointsGenerated - p.explosionsReceived * weight);
        return players.Where(p => Mathf.Approximately(p.pointsGenerated - p.explosionsReceived * weight, worst)).ToList();
    }
    #endregion
}
