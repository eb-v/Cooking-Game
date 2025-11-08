using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Ingredient")]
public class Ingredient : ScriptableObject
{
    [SerializeField] private string ingredientID; // Unique ID
    [SerializeField] private GameObject prefab;   // The actual prefab reference
    [SerializeField] private string ingredientName;
    public string IngredientID => ingredientID;
    public GameObject Prefab => prefab;
    public string IngredientName => ingredientName;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ingredientID))
            ingredientID = System.Guid.NewGuid().ToString();

        if (string.IsNullOrEmpty(ingredientName) && prefab != null)
            ingredientName = prefab.name;
    }

#endif
}


