using UnityEngine;
using static UISprings;

public class DampenedSpringEffect
{
    private DampedSpringMotionParams _motionParams;
    private float springPosValue;
    private float springVelValue;
    


    public virtual void RunSpringUpdateLogic(float angularFrequency, float dampingRatio, float multiplier, float goalValue)
    {
        float deltaTime = Time.deltaTime;
        CalcDampedSpringMotionParams(out _motionParams, deltaTime, angularFrequency, dampingRatio);
        UpdateDampedSpringMotion(ref springPosValue, ref springVelValue, goalValue, in _motionParams);
        OnSpringValueUpdated(springPosValue, multiplier);
    }


    public virtual void OnSpringValueUpdated(float springValue, float multiplier)
    {

    }
    
    

}



