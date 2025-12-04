using TMPro;
using UnityEngine;

public class TutorialGoal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    protected void CompleteGoal()
    {
        GenericEvent<AllTutorialGoalsCompleted>.GetEvent("TutorialManager").Invoke();
    }

}
