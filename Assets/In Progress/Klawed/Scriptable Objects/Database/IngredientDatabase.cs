using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientDatabase", menuName = "Database/IngredientDatabase")]
public class IngredientDatabase : ScriptableObject
{
    [SerializeField] private List<Ingredient> ingredients;

    private Dictionary<string, Ingredient> ingredientLookup;

    public Ingredient GetByID(string id)
    {
        EnsureLookup();
        ingredientLookup.TryGetValue(id, out var ingredient);
        return ingredient;
    }

    public Ingredient GetByName(string name)
    {
        EnsureLookup();
        foreach (var ingredient in ingredients)
        {
            if (ingredient.name == name)
                return ingredient;
        }
        return null;
    }

    private void EnsureLookup()
    {
        if (ingredientLookup == null)
        {
            ingredientLookup = new Dictionary<string, Ingredient>();
            foreach (var ingredient in ingredients)
                ingredientLookup[ingredient.IngredientID] = ingredient;
        }
    }

    public IReadOnlyList<Ingredient> AllIngredients => ingredients;
}
