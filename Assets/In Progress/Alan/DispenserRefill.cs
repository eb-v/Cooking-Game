using UnityEngine;

public class DispenserRefill : MonoBehaviour
{
    [Header("Which dispenser does this box feed?")]
    [SerializeField] private Dispenser targetDispenser;

    [Header("Refill Settings")]
    [SerializeField] private int refillAmount = 5;

    private IngredientType requiredType;

    private void Awake()
    {
        if (targetDispenser != null)
        {
            requiredType = targetDispenser.GetIngredientType();
        }
        else
        {
            Debug.LogWarning($"{name}: No targetDispenser assigned on DispenserSupplyBox.");
        }
    }

    // Ingredient object is dropped / thrown into the box trigger
    private void OnTriggerEnter(Collider other)
    {
        if (targetDispenser == null) return;

        // Look for IngredientTag on the object or its parent
        IngredientTag ingredient = other.GetComponent<IngredientTag>() 
                                   ?? other.GetComponentInParent<IngredientTag>();

        if (ingredient == null) return; // not an ingredient at all

        if (ingredient.type != requiredType)
        {
            Debug.Log($"{name}: Wrong ingredient ({ingredient.type}) for {requiredType} dispenser.");
            return;
        }

        // Correct ingredient -> refill
        targetDispenser.Refill(refillAmount);
        Debug.Log($"{name}: Refilled {targetDispenser.name} with {ingredient.type} ({refillAmount} uses).");

        // Consume the ingredient object
        Destroy(ingredient.gameObject);
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
