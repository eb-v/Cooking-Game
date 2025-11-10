using UnityEngine;

public class IngredientScript : MonoBehaviour
{
    [SerializeField] private Ingredient ingredient;

    public Ingredient Ingredient => ingredient;
}
