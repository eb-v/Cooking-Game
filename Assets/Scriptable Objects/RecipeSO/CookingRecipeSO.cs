using UnityEngine;

[CreateAssetMenu(fileName = "NewCookingRecipe", menuName = "Kitchen/Cooking Recipe")]
public class CookingRecipeSO : ScriptableObject {
    [Header("Cooking Recipe")]
    public GameObject input;
    public GameObject output;

    [Header("Cooking Properties")]
    public float cookingTime = 5f;
    public bool canBurn = false;
    public GameObject burntOutput;
}
