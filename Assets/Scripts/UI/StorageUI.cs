using TMPro;
using UnityEngine;

public class StorageUI : MonoBehaviour
{
    [SerializeField] private UDictionary<Ingredient, TMP_Text> ingredientCounterDisplay;

    private void Start()
    {
        GenericEvent<IngredientStorageAmountChanged>.GetEvent("AssemblyStation").AddListener(UpdateCounter);
    }

    private void UpdateCounter(Ingredient ingredient, int newCount)
    {
        if (ingredientCounterDisplay.ContainsKey(ingredient))
        {
            ingredientCounterDisplay[ingredient].text = "x" + newCount;
        }
    }

}
