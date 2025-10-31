using UnityEngine;
using UnityEngine.EventSystems;

public class SpringHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private SpringAPI spring;
    [SerializeField] private float hoverGoal = 1f;
    [SerializeField] private float defaultGoal = 0f;

    public void OnPointerEnter(PointerEventData eventData) {
        if (spring != null) {
            spring.SetGoalValue(hoverGoal);
            spring.NudgeSpringVelocity(); // gives it motion
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (spring != null) {
            spring.SetGoalValue(defaultGoal);
            spring.NudgeSpringVelocity();
        }
    }
}
