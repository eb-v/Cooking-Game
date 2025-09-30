using UnityEngine;

public class IngredientScript : MonoBehaviour
{
    public Ingredient ingredient { get; set; }

    [SerializeField] private GameObject preparedVersionPrefab;
    private bool isPrepared { get; set; }

    private void Awake()
    {
        ingredient = new Ingredient(gameObject, preparedVersionPrefab);
    }

    public void PrepareIngredient()
    {
        if (isPrepared)
        {
            Debug.LogWarning("Ingredient is already prepared: " + gameObject.name);
            return;
        }
        
        ingredient.SpawnPreparedVersion();
    }

   
}
