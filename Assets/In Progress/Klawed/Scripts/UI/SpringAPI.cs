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

    [SerializeField] private bool startAtEndPosition = false;

    public string eventChannel => gameObject.name;

    public string assignedEventChannel = "DefaultChannel";
    private string eventChannelName;

    public float nudgeStrength = 1f;

    [Header("Event Channel Name Options")]
    [SerializeField] private bool useGameObjectNameAsEventChannel = true;
    [SerializeField] private bool useCustomEventChannelName = false;
    [SerializeField] private bool useGameObjectIDAsEventChannel = false;
    private void Start()
    {
        if (startAtEndPosition)
        {
            springPosValue = maxGoal;
            goalValue = maxGoal;
        }
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
        InvokeEvent();
    }

    private void InvokeEvent()
    {
        switch (true)
        {
            case true when useGameObjectNameAsEventChannel:
                eventChannelName = gameObject.name;
                break;
            case true when useCustomEventChannelName:
                // assignedEventChannel is already set to custom name
                eventChannelName = assignedEventChannel;
                break;
            case true when useGameObjectIDAsEventChannel:
                eventChannelName = gameObject.GetInstanceID().ToString();
                break;
            default:
                eventChannelName = assignedEventChannel;
                break;
        }
        GenericEvent<SpringUpdateEvent>.GetEvent(eventChannelName).Invoke(springPosValue);
    }

    

    public void SetGoalValue(float value)
    {
        goalValue = Mathf.Clamp(value, minGoal, maxGoal);
    }

    public void NudgeSpringVelocity()
    {
        springVelValue = nudgeStrength;
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



