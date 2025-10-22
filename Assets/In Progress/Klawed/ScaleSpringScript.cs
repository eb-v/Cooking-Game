using UnityEngine;

public class ScaleSpringScript : MonoBehaviour
{
    private Scale_DampSpring scaleDampSpring;
    public float angularFrequency = 10f;
    public float dampingRatio = 0.7f;
    public float multiplier = 1f;
    private float goalValue = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scaleDampSpring = new Scale_DampSpring(transform);

        GenericEvent<UpdateGoalValueEvent>.GetEvent("me").AddListener(UpdateGoalValue);
    }

    // Update is called once per frame
    void Update()
    {
        scaleDampSpring.RunSpringUpdateLogic(angularFrequency, dampingRatio, multiplier, goalValue);
    }


    private void UpdateGoalValue(float goalValue)
    {
        this.goalValue = goalValue;
    }

}
