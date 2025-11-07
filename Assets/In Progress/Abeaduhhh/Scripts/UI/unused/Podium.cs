using UnityEngine;

public class PodiumController : MonoBehaviour {
    [Header("Animation")]
    public float baseHeight = 0f;
    public float maxHeight = 3f;
    public float riseSpeed = 2f;

    private float targetHeight;
    private Vector3 startPos;

    private void Awake() {
        startPos = transform.position;
        targetHeight = baseHeight;
    }

    private void Update() {
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, startPos.y + targetHeight, Time.deltaTime * riseSpeed);
        transform.position = pos;
    }

    public void SetNormalizedValue(float normalizedValue) {
        // normalizedValue between 0–1
        targetHeight = Mathf.Lerp(baseHeight, maxHeight, normalizedValue);
    }

    public void ResetHeight() {
        targetHeight = baseHeight;
    }
}
