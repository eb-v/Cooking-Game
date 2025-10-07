using UnityEngine;

// all ingredients should have this component
public class Ingredient : MonoBehaviour
{
    [SerializeField] private GameObject _ingredientPrefab;

    public GameObject IngredientPrefab => _ingredientPrefab;
    public string IngredientName => _ingredientPrefab.name;
}
