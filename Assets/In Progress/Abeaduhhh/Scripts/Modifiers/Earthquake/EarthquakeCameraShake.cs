using UnityEngine;
using System.Collections;

public class EarthquakeCameraShake : MonoBehaviour {
    private bool shaking = false;
    private Vector3 shakeOffset;

    public void Shake(float duration, float magnitude) {
        if (!shaking)
            StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude) {
        shaking = true;

        float timer = 0f;

        while (timer < duration) {
            shakeOffset.x = Random.Range(-1f, 1f) * magnitude;
            shakeOffset.y = Random.Range(-1f, 1f) * magnitude;

            timer += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        shaking = false;
    }

    void LateUpdate() {
        transform.localPosition += shakeOffset;
    }
}
