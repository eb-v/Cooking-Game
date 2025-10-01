using UnityEngine;

[CreateAssetMenu(fileName = "CircleSkillCheck", menuName = "Scriptable Objects/Kitchen Props/Skill Checks/Circle Skill Check")]
public class CircleSkillCheck : SkillCheckBaseLogic
{
   

    public override void Initialize(GameObject gameObject)
    {
        base.Initialize(gameObject);
    }

    public override void RunUpdateLogic()
    {
        // display logic


        // success logic
        if (skillChecksCompleted >= skillChecksRequired)
        {
            IPrepStation prepStation = gameObject.GetComponent<IPrepStation>();
            GenericEvent<SkillCheckCompleted>.GetEvent(gameObject.name).Invoke();
        }
    }

    public override void DoAttemptSkillCheckLogic()
    {
        skillChecksCompleted++;
        Debug.Log("Circle Skill Check Attempted. Total Completed: " + skillChecksCompleted);
        if (skillChecksCompleted >= skillChecksRequired)
        {
            IPrepStation prepStation = gameObject.GetComponent<IPrepStation>();
            prepStation.containsObject = false; // free up the station
            GenericEvent<SkillCheckCompleted>.GetEvent(gameObject.name).Invoke();
            skillChecksCompleted = 0; // reset for next time
        }
    }

   
}
