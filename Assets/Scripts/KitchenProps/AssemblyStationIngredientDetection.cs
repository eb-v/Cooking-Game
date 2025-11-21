using UnityEngine;

// the object that holds this script should hold a trigger collider that only detects objects on the Ingredient layer
public class AssemblyStationIngredientDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GenericEvent<AssemblyStationColliderEntered>.GetEvent("AssemblyStation").Invoke(other.transform.root.gameObject);
    }

}
