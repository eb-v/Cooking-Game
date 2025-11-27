using UnityEngine;

public class EarthquakeCameraShake : MonoBehaviour {
    private Vector3 originalPos;
    private bool shaking = false;

    void Start() {
        originalPos = transform.localPosition;
    }

    public void Shake(float duration, float magnitude) {
        if (!shaking)
            StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private System.Collections.IEnumerator ShakeRoutine(float duration, float magnitude) {
        shaking = true;

        float timer = 0f;

        while (timer < duration) {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        // reset camera
        transform.localPosition = originalPos;
        shaking = false;
    }
}
