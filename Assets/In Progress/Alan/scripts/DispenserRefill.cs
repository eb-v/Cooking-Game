using UnityEngine;

public class DispenserRefill : MonoBehaviour
{
    [Header("Which dispenser does this box feed?")]
    [SerializeField] private Dispenser targetDispenser;

    [Header("Refill Settings")]
    [SerializeField] private int refillAmount = 5;

    [Header("Ingredient (auto from dispenser)")]
    [SerializeField] private Ingredient requiredIngredient;

    private void Awake()
    {
        if (targetDispenser == null)
        {
            Debug.LogWarning($"{name}: No targetDispenser assigned on DispenserRefill.");
            return;
        }

        // auto pull from the dispenser if not manually set
        if (requiredIngredient == null)
        {
            requiredIngredient = targetDispenser.Ingredient;
        }

        if (requiredIngredient == null)
        {
            Debug.LogWarning(
                $"{name}: targetDispenser {targetDispenser.name} has no Ingredient assigned."
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetDispenser == null || requiredIngredient == null)
            return;

        IngredientScript ingredientScript =
            other.GetComponent<IngredientScript>() ??
            other.GetComponentInParent<IngredientScript>();

        if (ingredientScript == null || ingredientScript.Ingredient == null)
            return; // not an ingredient refill object

        if (ingredientScript.Ingredient != requiredIngredient)
        {
            Debug.Log(
                $"{name}: Wrong ingredient ({ingredientScript.Ingredient.name}) " +
                $"for dispenser {targetDispenser.name} (needs {requiredIngredient.name})."
            );
            return;
        }

        targetDispenser.Refill(refillAmount);
        Debug.Log(
            $"{name}: Refilled {targetDispenser.name} with {requiredIngredient.name} " +
            $"(+{refillAmount} uses)."
        );

        Destroy(ingredientScript.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
