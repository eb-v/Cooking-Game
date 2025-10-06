using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager: MonoBehaviour {

    [SerializeField] private RecipeListSO recipeListSO;

    public List<RecipeSO> WaitingRecipes => waitingRecipeSOList;
    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    
    //change max after testing
    private int waitingRecipesMax = 10;

    private void Awake() {
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Start() {
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    private void Update() {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipesMax) {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
                Debug.Log(waitingRecipeSO.recipeName);
                waitingRecipeSOList.Add(waitingRecipeSO);
            }

        }
    }

    public bool MatchesRecipe(AssembledItemObject assembledItem, RecipeSO recipe){
        List<GameObject> assembledPrefabs = assembledItem.GetIngredients();

        foreach (CuttingRecipeSO cuttingRecipe in recipe.CuttingRecipeSOList)
        {
            if (!assembledPrefabs.Contains(cuttingRecipe.output))
            {
                Debug.Log("missing a cutting ingredient");
                return false; 
            }
        }

        foreach (CookingRecipeSO cookingRecipe in recipe.CookingRecipeSOList)
        {
            if (!assembledPrefabs.Contains(cookingRecipe.output))
            {
                Debug.Log("missing a cooking ingredient");
                return false; 
            }
        }

        Debug.Log("All ingredients match recipe!");
        return true;
    }

}