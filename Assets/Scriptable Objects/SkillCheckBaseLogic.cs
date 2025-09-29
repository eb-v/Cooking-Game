using UnityEngine;

public class SkillCheckBaseLogic : ScriptableObject
{
    protected GameObject gameObject;
    public int skillChecksRequired;
    public int skillChecksCompleted;


    public virtual void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }


    public virtual void RunUpdateLogic()
    {
        Debug.Log("Base Skill Check Logic");
    }

    public virtual void DoAttemptSkillCheckLogic()
    {
        Debug.Log("Base Attempt Skill Check Logic");
    }
}
