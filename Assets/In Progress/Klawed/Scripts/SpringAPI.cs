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

    private void Update()
    {
        float springPosValue = CalculateSpringValue();
        OnUpdateSpring(springPosValue);
    }

    private float CalculateSpringValue()
    {
        float deltaTime = Time.deltaTime;
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
        // for most cases, this should be clamped between 0 and 1
        goalValue = Mathf.Clamp(value, minGoal, maxGoal);
    }

    // this is called to create a visual bounce effect if needed for a ui element
    public void NudgeSpringVelocity()
    {
        springVelValue += nudgeStrength;
    }

    public void ResetSpring()
    {
        springPosValue = 0f;
        springVelValue = 0f;
    }


}



