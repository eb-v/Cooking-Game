using UnityEngine;

public class EarthquakeModifier : MonoBehaviour {
    public bool enabledInThisLevel = false;

    public EarthquakeController earthquake;

    [Header("Timing")]
    public float minInterval = 5f;
    public float maxInterval = 15f;

    private float timer;

    void Start() {

        enabledInThisLevel = true;

        earthquake.ActivateEarthquake();
        ResetTimer();
    }

    void Update() {
        if (!enabledInThisLevel) return;

        timer -= Time.deltaTime;
        if (timer <= 0f) {
            earthquake.ActivateEarthquake();
            ResetTimer();
        }
    }

    void ResetTimer() {
        timer = Random.Range(minInterval, maxInterval);
    }

    public void EnableModifier() {
        enabledInThisLevel = true;
        ResetTimer();
    }
}