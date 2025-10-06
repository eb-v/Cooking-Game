using System.Collections.Generic;
using UnityEngine;

public class RecipeTest : MonoBehaviour
{
    [SerializeField] private DeliveryManager deliveryManager;    
    [SerializeField] private GameObject cookingOutputPrefab;      
    [SerializeField] private GameObject cuttingOutputPrefab;      

    private void Start()
    {
        if (deliveryManager == null)
        {
            Debug.LogError("DeliveryManager reference not set!");
            return;
        }

        AssembledItemObject assembledItem = new AssembledItemObject();

        // Add test ingredients
        assembledItem.AddIngredient(cookingOutputPrefab);
        assembledItem.AddIngredient(cuttingOutputPrefab);

        bool matchedAny = false;
        foreach (RecipeSO recipe in deliveryManager.WaitingRecipes)
        {
            if (deliveryManager.MatchesRecipe(assembledItem, recipe))
            {
                Debug.Log("Assembled item matches order: " + recipe.recipeName);
                matchedAny = true;
                break;
            }
        }

        if (!matchedAny)
        {
            Debug.Log("Assembled item does NOT match any active order.");
        }
    }
}
