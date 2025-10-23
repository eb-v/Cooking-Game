using UnityEngine;
using static UISprings;

public class SpringAPI
{
    private DampedSpringMotionParams _motionParams;
    private float springPosValue;
    private float springVelValue;
    


    public float CalculateSpringValue(float angularFrequency, float dampingRatio, float multiplier, float goalValue)
    {
        float deltaTime = Time.deltaTime;
        CalcDampedSpringMotionParams(out _motionParams, deltaTime, angularFrequency, dampingRatio);
        UpdateDampedSpringMotion(ref springPosValue, ref springVelValue, goalValue, in _motionParams);

        return springPosValue;
    }



}



