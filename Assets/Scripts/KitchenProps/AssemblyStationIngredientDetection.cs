using UnityEngine;

// the object that holds this script should hold a trigger collider that only detects objects on the Ingredient layer
public class AssemblyStationIngredientDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GenericEvent<IngredientEnteredAssemblyArea>.GetEvent(transform.root.name).Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        GenericEvent<IngredientExitedAssemblyArea>.GetEvent(transform.root.name).Invoke(other.gameObject);
    }
}
