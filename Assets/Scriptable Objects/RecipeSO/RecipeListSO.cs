using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeListSO", menuName = "Scriptable Objects/Recipe/Recipe List SO")]
public class RecipeListSO : ScriptableObject {
    
    public List<RecipeSO> recipeSOList;

}