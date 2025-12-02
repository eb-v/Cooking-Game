using UnityEngine;

public class EarthquakeModifier : MonoBehaviour {
    [Header("Modifier Settings")]
    public bool enabledInThisLevel = false;
    public EarthquakeController earthquake;

    [Header("Timing")]
    public float minInterval = 5f;
    public float maxInterval = 15f;

    private float timer;

    void Start() {
        if (enabledInThisLevel && earthquake != null) {
            ResetTimer();
        }
    }

    private void OnEnable()
    {
        if (enabledInThisLevel && earthquake != null)
        {
            ResetTimer();
        }
    }

    void Update() {
        if (!enabledInThisLevel || earthquake == null) return;

        timer -= Time.deltaTime;

        if (timer <= 0f) {
            StartEarthquake();
            ResetTimer();
        }
    }

    private void StartEarthquake() {
        if (earthquake != null) {
            earthquake.ActivateEarthquake();
        }
    }

    private void ResetTimer() {
        timer = Random.Range(minInterval, maxInterval);
    }

    public void EnableModifier() {
        enabledInThisLevel = true;
        ResetTimer();
    }

    public void DisableModifier() {
        enabledInThisLevel = false;
    }
}
