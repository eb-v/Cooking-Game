using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class PizzaDoughBase : MonoBehaviour
{
    [SerializeField] private List<GameObject> currentPlacedIngredientPrefabs = new List<GameObject>();
    [SerializeField] private List<Transform> ingredientContainers;
    [SerializeField] private List<GameObject> ingredientObjectInstances;

    public bool CheckForIngredient(GameObject ingredientPrefab)
    {
        return currentPlacedIngredientPrefabs.Contains(ingredientPrefab);
    }

    public void AddIngredient(GameObject ingredientPrefab)
    {
        currentPlacedIngredientPrefabs.Add(ingredientPrefab);
        if (ingredientPrefab.name.Contains("Tomato"))
        {
            Transform transform = ingredientContainers.Find(t => t.name == "TomatoSauceContainer");
            GameObject ingredientInstance = ObjectPoolManager.SpawnObject(ingredientPrefab, transform.position, Quaternion.identity);
            ingredientObjectInstances.Add(ingredientInstance);
            ingredientInstance.transform.SetParent(transform);

        }
        else if (ingredientPrefab.name.Contains("Cheese"))
        {
            Transform transform = ingredientContainers.Find(t => t.name == "GratedCheeseContainer");
            GameObject ingredientInstance = ObjectPoolManager.SpawnObject(ingredientPrefab, transform.position, Quaternion.identity);
            ingredientObjectInstances.Add(ingredientInstance);
            ingredientInstance.transform.SetParent(transform);
        }
        else if (ingredientPrefab.name.Contains("Pepperoni"))
        {
            Transform transform = ingredientContainers.Find(t => t.name == "PepperoniContainer");
            GameObject ingredientInstance = ObjectPoolManager.SpawnObject(ingredientPrefab, transform.position, Quaternion.identity);
            ingredientObjectInstances.Add(ingredientInstance);  
            ingredientInstance.transform.SetParent(transform);
        }
        else
        {
            Debug.LogWarning("Ingredient prefab does not match any known ingredient containers.");
        }
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
