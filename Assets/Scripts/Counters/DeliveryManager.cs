using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryManager: MonoBehaviour {

    [SerializeField] private RecipeListSO recipeListSO;

    public List<RecipeSO> WaitingRecipes => waitingRecipeSOList;
    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    
    //change max after testing
    private int waitingRecipesMax = 4;
    public UnityEvent OnRecipeListChanged;

    private void Awake() {
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Start() {
        Random.InitState(System.DateTime.Now.Millisecond);

        spawnRecipeTimer = 0.15f;
    }

    private void Update() {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipesMax) {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
                Debug.Log(waitingRecipeSO.recipeName);
                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeListChanged?.Invoke();
            }

        }
    }

    private bool MatchesRecipe(AssembledItemObject assembledItem, RecipeSO recipe)
    {
        List<GameObject> assembledIngredients = assembledItem.GetIngredients();

        foreach (CuttingRecipeSO cuttingRecipe in recipe.CuttingRecipeSOList)
        {
            bool found = assembledIngredients.Exists(go => go != null && go.name.Contains(cuttingRecipe.output.name));
            if (!found)
            {
                Debug.Log("Missing cutting ingredient for recipe " + recipe.recipeName);
                return false;
            }
        }

        foreach (CookingRecipeSO cookingRecipe in recipe.CookingRecipeSOList)
        {
            bool found = assembledIngredients.Exists(go => go != null && go.name.Contains(cookingRecipe.output.name));
            if (!found)
            {
                Debug.Log("Missing cooking ingredient for recipe " + recipe.recipeName);
                return false;
            }
        }
        return true;
    }


    public bool TryMatchAndDeliver(GameObject assembledGO) 
    {
        AssembledItemObject assembledItem = assembledGO.GetComponent<AssembledItemObject>();
        if (assembledItem == null)
        {
            Debug.LogError("Held object has no AssembledItemObject!");
            return false;
        }

        foreach (RecipeSO recipe in waitingRecipeSOList) {
            if (MatchesRecipe(assembledItem, recipe)) {
                Debug.Log("Delivered: " + recipe.recipeName);

                // destroy gameobject after 1 sec
                GameObject.Destroy(assembledGO, 1f);

                // update scoring
                if (PointManager.Instance != null)
                    PointManager.Instance.AddDeliveredDish();
                else
                    Debug.LogError("PointManager.Instance is null!");

                waitingRecipeSOList.Remove(recipe);


                OnRecipeListChanged?.Invoke();

                return true;
            }
        }

        Debug.Log("Assembled item does not match any active order.");
        return false;
    }

}