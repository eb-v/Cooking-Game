using UnityEngine;
using UnityEngine.EventSystems;

public class ShakeOnHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [Header("Target to Shake")]
    [Tooltip("Assign the UI element (RectTransform) that should shake. " +
             "For example, the Text or Icon of a Button.")]
    [SerializeField] private RectTransform shakeTarget;

    [Header("Shake Settings")]
    [Tooltip("How far the object moves while shaking.")]
    [SerializeField] private float shakeAmount = 5f;

    [Tooltip("How fast the shaking oscillates.")]
    [SerializeField] private float shakeSpeed = 25f;

    [Tooltip("How quickly it returns to normal when hover ends.")]
    [SerializeField] private float returnSpeed = 10f;

    private bool isHovering = false;
    private Vector3 originalPos;

    private void Start() {
        if (shakeTarget == null)
            shakeTarget = GetComponent<RectTransform>();

        originalPos = shakeTarget.localPosition;
    }

    private void Update() {
        if (isHovering) {
            float x = Mathf.Sin(Time.unscaledTime * shakeSpeed) * shakeAmount;
            float y = Mathf.Cos(Time.unscaledTime * shakeSpeed) * shakeAmount;
            shakeTarget.localPosition = originalPos + new Vector3(x, y, 0);
        } else {
            shakeTarget.localPosition = Vector3.Lerp(
                shakeTarget.localPosition,
                originalPos,
                Time.unscaledDeltaTime * returnSpeed
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovering = false;
    }
}
