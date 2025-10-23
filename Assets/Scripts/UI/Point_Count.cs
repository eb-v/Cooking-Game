using TMPro;
using UnityEngine;
using System.Collections;

public class DeliveredCountUI : MonoBehaviour {
    [SerializeField] private TMP_Text deliveredText;
    [SerializeField] private float animationDuration = 0.5f; // How long the animation takes
    
    private int totalPoints = 0;
    private int displayedPoints = 0;
    private Coroutine countingCoroutine;
    
    private void Start() {
        if (PointManager.Instance != null) {
            UpdateUI(PointManager.Instance.GetDeliveredCount());
            PointManager.Instance.OnDishDelivered.AddListener(UpdateUI);
        }
        
        // Initialize display
        deliveredText.text = "Points: 0000";
    }
    
    private void UpdateUI(int ingredientCount) {
        totalPoints += ingredientCount * 100;
        
        // Stop any existing animation
        if (countingCoroutine != null) {
            StopCoroutine(countingCoroutine);
        }
        
        // Start new animation
        countingCoroutine = StartCoroutine(AnimateCounter(displayedPoints, totalPoints));
    }
    
    private IEnumerator AnimateCounter(int startValue, int endValue) {
        float elapsed = 0f;
        
        while (elapsed < animationDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Use easing for smoother animation (optional - you can use linear with just 't')
            float easedT = Mathf.SmoothStep(0, 1, t);
            
            displayedPoints = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, easedT));
            deliveredText.text = "Points: " + displayedPoints.ToString("D4"); // D4 formats as 4 digits with leading zeros
            
            yield return null;
        }
        
        // Ensure we end at exact value
        displayedPoints = endValue;
        deliveredText.text = "Points: " + displayedPoints.ToString("D4");
    }
    
    private void OnDestroy() {
        if (PointManager.Instance != null)
            PointManager.Instance.OnDishDelivered.RemoveListener(UpdateUI);
    }
}