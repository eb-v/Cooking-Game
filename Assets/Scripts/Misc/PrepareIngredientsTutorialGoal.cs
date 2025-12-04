using UnityEngine;

public class PrepareIngredientsTutorialGoal : TutorialGoal
{
    [SerializeField] private int ingredientsToCut = 3;
    [SerializeField] private int ingredientsToCook = 2;

    private void OnEnable()
    {
        GenericEvent<TutorialGoalCompleted>.GetEvent("IngredientCut").AddListener(OnIngredientCut);
        GenericEvent<TutorialGoalCompleted>.GetEvent("IngredientCooked").AddListener(OnIngredientCooked);
    }

    private void OnDisable()
    {
        GenericEvent<TutorialGoalCompleted>.GetEvent("IngredientCut").RemoveListener(OnIngredientCut);
        GenericEvent<TutorialGoalCompleted>.GetEvent("IngredientCooked").RemoveListener(OnIngredientCooked);
    }

    private void OnIngredientCut()
    {
        ingredientsToCut--;
        if (GoalCompleted())
        {
            CompleteGoal();
        }
    }

    private void OnIngredientCooked()
    {
        ingredientsToCook--;
        if (GoalCompleted())
        {
            CompleteGoal();
        }
    }

    private bool GoalCompleted()
    {
        return ingredientsToCut <= 0 && ingredientsToCook <= 0;
    }


}
