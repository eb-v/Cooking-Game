using UnityEngine;
using static UISprings;

public class SpringAPI : MonoBehaviour
{
    private DampedSpringMotionParams _motionParams;
    private float springPosValue;
    private float springVelValue;
    private float goalValue;

    public float angularFrequency = 10f;
    public float dampingRatio = 0.7f;
    public float multiplier = 1f;
    public float maxGoal = 1f;
    public float minGoal = 0f;

    public float nudgeStrength = 1f;

    private void Start()
    {
        GenericEvent<OnButtonPressedEvent>.GetEvent("me").AddListener(NudgeSpringVelocity); 
    }

    private void Update()
    {
        float springPosValue = CalculateSpringValue();
        OnUpdateSpring(springPosValue);
    }

    public float CalculateSpringValue()
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

    
}



