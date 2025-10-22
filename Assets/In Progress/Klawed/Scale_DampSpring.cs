using static UISprings;
using UnityEngine;

public class Scale_DampSpring : DampenedSpringEffect
{
    public Transform transform;
    

    public Scale_DampSpring(Transform transform)
    {
        this.transform = transform;
    }

    public override void RunSpringUpdateLogic(float angularFrequency, float dampingRatio, float multiplier, float goalValue)
    {
        base.RunSpringUpdateLogic(angularFrequency, dampingRatio, multiplier, goalValue);
    }


    public override void OnSpringValueUpdated(float springValue, float multiplier)
    {
        float finalSpringValue = Mathf.Abs(springValue) * multiplier;
        
        transform.localScale = Vector3.one * finalSpringValue;
    }



}
