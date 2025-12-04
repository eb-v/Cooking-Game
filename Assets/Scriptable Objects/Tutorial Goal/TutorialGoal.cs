using UnityEngine;

public class TutorialGoal : MonoBehaviour
{
    [SerializeField] private string goalDescription;



    protected void CompleteGoal()
    {
        GenericEvent<AllTutorialGoalsCompleted>.GetEvent("TutorialManager").Invoke();
    }

}
