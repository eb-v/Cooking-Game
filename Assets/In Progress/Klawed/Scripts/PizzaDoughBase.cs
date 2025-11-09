using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class PizzaDoughBase : MonoBehaviour
{
    [SerializeField] private List<Ingredient> currentPlacedIngredients = new List<Ingredient>();
    [SerializeField] private Transform ingredientContainer;
    [SerializeField] private List<GameObject> ingredientObjectInstances;

    public bool CheckForIngredient(Ingredient ingredient)
    {
        return currentPlacedIngredients.Contains(ingredient);
    }

    public void AddIngredient(Ingredient ingredient)
    {
        currentPlacedIngredients.Add(ingredient);
        GameObject ingredientInstance = ObjectPoolManager.SpawnObject(ingredient.Prefab, ingredientContainer, Quaternion.identity);
        ingredientObjectInstances.Add(ingredientInstance);
    }

    public List<GameObject> GetIngredientInstancesOnPizza()
    {
        return ingredientObjectInstances;
    }

    public void ClearIngredientInstanceList()
    {
        ingredientObjectInstances.Clear();
    }


}
