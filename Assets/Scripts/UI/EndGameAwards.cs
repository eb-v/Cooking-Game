using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameAwards : MonoBehaviour {
    [Header("Award Display")]
    [SerializeField] private GameObject awardDisplayPanel;
    [SerializeField] private TextMeshProUGUI awardTitleText;
    [SerializeField] private TextMeshProUGUI awardDescriptionText;
    [SerializeField] private TextMeshProUGUI playerNamesText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private float displayDuration = 4f;
    [SerializeField] private GameOverButtonsController buttonsController;

    [Header("Camera")]
    [SerializeField] private Camera awardCamera;
   // [SerializeField] private float rotationSpeed = 30f;

    [Header("Spring Animations")]
    [SerializeField] private float springDelayBetweenTexts = 0f;
    private List<PlayerStatsData> players = new List<PlayerStatsData>();
    private bool ceremonyCancelled = false;

    public static EndGameAwards Instance { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Coroutine ShowAwards(MonoBehaviour caller)
    {
        Debug.Log("ShowAwards called");

        // Get all player data and store it locally
        players = PlayerStatsManager.GetAllPlayersData();
        Debug.Log($"Retrieved {players?.Count ?? 0} players");

        if (players == null || players.Count == 0)
        {
            Debug.LogWarning("No players found in PlayerStatsManager. Cannot show awards.");
            return null;
        }

        Debug.Log($"Starting awards ceremony with {players.Count} players");
        
        // Debug log each player's stats
        foreach (PlayerStatsData playerStats in players)
        {
            Debug.Log($"{playerStats.player.name}: Points={playerStats.pointsGenerated}, Items={playerStats.itemsGrabbed}, Joints={playerStats.jointsReconnected}, Explosions={playerStats.explosionsReceived}");
        }
        
        // Clear the stats manager immediately after retrieving the data
        // This ensures fresh stats for the next level
        PlayerStatsManager.ClearAllPlayers();
        Debug.Log("PlayerStatsManager cleared for next level");
        
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

        // Awards
        yield return ShowAward("Hot Hands", "Most Items Grabbed",
            GetMaxStatWinners(p => p.itemsGrabbed),
            p => $"Items: {p.itemsGrabbed}");
        if (ceremonyCancelled) yield break;

        yield return ShowAward("MVP", "Most Points Generated",
            GetMaxStatWinners(p => p.pointsGenerated),
            p => $"Points: {p.pointsGenerated}");
        if (ceremonyCancelled) yield break;

        var guardianWinners = GetMaxStatWinners(p => p.jointsReconnected, requireNonZero: true);
        if (guardianWinners.Count > 0)
            yield return ShowAward("Guardian Angel", "Most Joints Reconnected",
                guardianWinners, p => $"Joints: {p.jointsReconnected}");
        if (ceremonyCancelled) yield break;

        yield return ShowAward("Noob", "Worst Performance (Low Points + High Explosions)",
            GetNoobWinners(),
            p => $"Points: {p.pointsGenerated} | Explosions: {p.explosionsReceived}");

        buttonsController?.ShowButtons();

        yield return new WaitForSecondsRealtime(1f);
        awardDisplayPanel.SetActive(false);
    }

    private IEnumerator ShowAward(
      string title,
      string description,
      List<PlayerStatsData> winners,
      System.Func<PlayerStatsData, string> getStatText) {
        if (winners == null || winners.Count == 0) yield break;

        awardTitleText.text = title + (winners.Count > 1 && title != "Hot Hands" ? "s" : "");
        awardDescriptionText.text = description;
        playerNamesText.text = string.Join(" & ", winners.Select(p => $"{p.player.name}"));
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
    private List<PlayerStatsData> GetMaxStatWinners(System.Func<PlayerStatsData, int> selector, bool requireNonZero = false) {
        if (players.Count == 0) return new List<PlayerStatsData>();
        
        int max = players.Max(selector);
        if (requireNonZero && max <= 0) return new List<PlayerStatsData>();
        return players.Where(p => selector(p) == max).ToList();
    }

    private List<PlayerStatsData> GetNoobWinners() {
        if (players.Count == 0) return new List<PlayerStatsData>();
        
        int weight = 10;
        float worst = players.Min(p => p.pointsGenerated - p.explosionsReceived * weight);
        return players.Where(p => Mathf.Approximately(p.pointsGenerated - p.explosionsReceived * weight, worst)).ToList();
    }
    #endregion
}