using UnityEngine;
using static UISprings;

public class DampenedSpring : MonoBehaviour
{
    DampedSpringMotionParams motionParams, scaleParamsX, scaleParamsY, scaleParamsZ;
    public float angularFrequency = 10f;
    public float dampingRatio = 0.5f;
    public float scaleMultiplier = 2f;
    public float springPosValue, springVelValue;
    public float goalPos;
    public float finalSpringValue;
    public float startingScale = 1f;
    public float posMultiplier = 80f;
    public Vector3 startingPos;
    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    private void Start()
    {
        GenericEvent<MoveImageEvent>.GetEvent("me").AddListener(MoveImage);
    }


    private void Update()
    {
        float deltaTime = Time.deltaTime;
        CalcDampedSpringMotionParams(out motionParams, deltaTime, angularFrequency, dampingRatio);
        //CalcDampedSpringMotionParams(out motionParams, deltaTime, angularFrequency, dampingRatio);
        //CalcDampedSpringMotionParams(out motionParams, deltaTime, angularFrequency, dampingRatio);

       

        //UpdateDampedSpringMotion(ref pos.x, ref vel.x, goalPos.x, in motionParams);
        UpdateDampedSpringMotion(ref springPosValue, ref springVelValue, goalPos, in motionParams);


        //Debug.Log("Position: " + pos + " Scale: " + scale);
        
        //rectTransform.position = pos;
        OnSpringValueUpdated(springPosValue);

    }


    
    public void OnSpringValueUpdated(float springValue)
    {
        // update scale
        finalSpringValue = Mathf.Abs(springValue) * scaleMultiplier;
        rectTransform.localScale = Vector3.one * finalSpringValue;


        // update position
        finalSpringValue = springValue * posMultiplier;
        rectTransform.localPosition = new Vector3(finalSpringValue, finalSpringValue, 0f);
    }



    public void MoveImage(bool isPressed)
    {
        if (isPressed)
        {
            goalPos = 1f;
        }
        else
        {
            goalPos = 0f;
        }
    }

   
}
