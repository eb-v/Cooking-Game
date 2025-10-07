using TMPro;
using UnityEngine;

public class DeliveredCountUI : MonoBehaviour {
    [SerializeField] private TMP_Text deliveredText;

    private void Start() {
        if (PointManager.Instance != null) {
            UpdateUI(PointManager.Instance.GetDeliveredCount());

            PointManager.Instance.OnDishDelivered.AddListener(UpdateUI);
        }
    }

    private void UpdateUI(int count) {
        deliveredText.text = "Delivered: " + count;
    }

    private void OnDestroy() {
        if (PointManager.Instance != null)
            PointManager.Instance.OnDishDelivered.RemoveListener(UpdateUI);
    }
}
