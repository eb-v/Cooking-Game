using UnityEngine;
using UnityEngine.UI;

public class OrderCardUI : MonoBehaviour {
    [SerializeField] private Image recipeImage; // this must be a unique Image for each prefab instance

    public void SetRecipe(RecipeSO recipe) {
        if (recipe != null && recipeImage != null) {
            recipeImage.sprite = recipe.recipeSprite; // uses the sprite from this recipe only
        }
    }
}
