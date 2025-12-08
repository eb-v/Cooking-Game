using UnityEngine;
using static UISprings;

// this is a modified version of SpringAPI that does not normalize the goal value between 0 and 1
public class NonNormalizedSpringAPI : MonoBehaviour
{
    private DampedSpringMotionParams _motionParams;
    [SerializeField] private float springPosValue;
    [SerializeField] private float springVelValue;
    public float goalValue = 0f;

    public float angularFrequency = 10f;
    public float dampingRatio = 0.7f;

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
        float deltaTime = Time.unscaledDeltaTime;

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
        goalValue = value;
    }

    // this is called to create a visual bounce effect if needed for a ui element
    public void NudgeSpringVelocity()
    {
        springVelValue += nudgeStrength;
    }

    public void SetVelocity(float velocity)
    {
        springVelValue = velocity;
    }

    public void ResetPosition()
    {
        springPosValue = 0f;
    }

    public void ResetVelocity()
    {
        springVelValue = 0f;
    }

    public float GetPosition()
    {
        return springPosValue;
    }

    public float GetVelocity()
    {
        return springVelValue;
    }
}
