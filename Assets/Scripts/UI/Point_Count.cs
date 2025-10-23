using TMPro;
using UnityEngine;

public class DeliveredCountUI : MonoBehaviour {
    [SerializeField] private TMP_Text deliveredText;
    private int totalPoints = 0;

    private void Start() {
        if (PointManager.Instance != null) {
            UpdateUI(PointManager.Instance.GetDeliveredCount());
            PointManager.Instance.OnDishDelivered.AddListener(UpdateUI);
        }
    }

    private void UpdateUI(int ingredientCount) {
        totalPoints += ingredientCount * 100;
        deliveredText.text = "Points: " + totalPoints;
    }

    private void OnDestroy() {
        if (PointManager.Instance != null)
            PointManager.Instance.OnDishDelivered.RemoveListener(UpdateUI);
    }
}