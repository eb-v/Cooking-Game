using UnityEngine;
using static UISprings;

public class SpringAPI : MonoBehaviour, IEventChannel
{
    private DampedSpringMotionParams _motionParams;
    private float springPosValue;
    private float springVelValue;
    public float goalValue = 0f;

    public float angularFrequency = 10f;
    public float dampingRatio = 0.7f;
    public float maxGoal = 1f;
    public float minGoal = 0f;

    public string eventChannel => gameObject.name;

    public float nudgeStrength = 1f;

    private void Start()
    {
    }
    private void Update() {
        float deltaTime = Time.unscaledDeltaTime;
        springPosValue = CalculateSpringValue(deltaTime);
        OnUpdateSpring(springPosValue);
    }

    private float CalculateSpringValue(float deltaTime) {
        CalcDampedSpringMotionParams(out _motionParams, deltaTime, angularFrequency, dampingRatio);
        UpdateDampedSpringMotion(ref springPosValue, ref springVelValue, goalValue, in _motionParams);
        return springPosValue;
    }


    private void OnUpdateSpring(float springPosValue)
    {
        GenericEvent<SpringUpdateEvent>.GetEvent(gameObject.name).Invoke(springPosValue);
    }

    

    public void SetGoalValue(float value)
    {
        goalValue = Mathf.Clamp(value, minGoal, maxGoal);
    }

    public void NudgeSpringVelocity()
    {
        springVelValue += nudgeStrength;
    }

    public void ResetSpring()
    {
        springPosValue = 0f;
        springVelValue = 0f;
    }

    public void PlaySpring() {
        ResetSpring();

        SetGoalValue(maxGoal);

        NudgeSpringVelocity();
    }

    public void HideSpring() {
        SetGoalValue(minGoal);
    }

}



