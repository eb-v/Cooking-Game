using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "Scriptable Objects/RecipeSO")]
public class RecipeSO : ScriptableObject {
    public List<BaseKitchenObjectSO> BaseKitchenSOList; //change to cuttingrecipeSO and other prepared ingredients, not base ingredients...
    public List<CookingRecipeSO> CookingRecipeSOList;
    public List<CuttingRecipeSO> CuttingRecipeSOList;

    public Sprite recipeSprite;
    public Sprite finalProductImageUI;
    public string recipeName;
    public GameObject finalProductPrefab;
}