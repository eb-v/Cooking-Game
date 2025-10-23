using UnityEngine;

public class ScaleSpringScript : MonoBehaviour
{
    public float angularFrequency = 10f;
    public float dampingRatio = 0.7f;
    public float multiplier = 1f;
    public float baseScale = 1f;
    private float goalValue = 0.0f; 


    void Start()
    {
        GenericEvent<UpdateGoalValueEvent>.GetEvent("me").AddListener(UpdateGoalValue);
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void UpdateGoalValue(float goalValue)
    {
        this.goalValue = goalValue;
    }

}
