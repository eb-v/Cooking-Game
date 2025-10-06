using UnityEngine;
using TMPro;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;

    private void Start()
    {
        if (countdownText == null) countdownText = GetComponent<TMP_Text>();

        // subscribe safely
        if (KitchenGameManager.Instance != null)
            KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        UpdateVisibility();
    }

    private void OnDestroy()
    {
        if (KitchenGameManager.Instance != null)
            KitchenGameManager.Instance.OnStateChanged -= KitchenGameManager_OnStateChanged;
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        UpdateVisibility();
    }

    private void Update()
    {
        if (KitchenGameManager.Instance == null) return;

        if (KitchenGameManager.Instance.IsCountdownToStartActive())
        {
            float t = KitchenGameManager.Instance.GetCountdownToStartTimer();
            countdownText.text = Mathf.CeilToInt(t).ToString();   // shows 3..2..1
        }
    }

    private void UpdateVisibility()
    {
        bool show = KitchenGameManager.Instance != null &&
                    KitchenGameManager.Instance.IsCountdownToStartActive();
        gameObject.SetActive(show);
    }
}
