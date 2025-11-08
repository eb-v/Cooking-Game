using UnityEngine;

public class IngredientScript : MonoBehaviour
{
    [SerializeField] private string ingredientID;  // Assigned when spawned

    public string IngredientID => ingredientID;

    public void Initialize(Ingredient ingredient)
    {
        ingredientID = ingredient.IngredientID;
    }
}
