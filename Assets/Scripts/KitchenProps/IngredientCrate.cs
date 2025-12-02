using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class IngredientCrate : MonoBehaviour
{
    [SerializeField] private Ingredient ingredientSO;
    [SerializeField] private Transform spawnTransform;

    private void OnEnable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnInteract);
    }

    public void OnInteract(GameObject player)
    {
        GrabScript grabScript = player.GetComponent<GrabScript>();
        if (grabScript.IsGrabbing)
            return;


        Transform playerRoot = player.GetComponent<RagdollController>().GetPelvis().transform;
        Vector3 direction = (spawnTransform.position - playerRoot.position).normalized;

        SpawnIngredientInPlayersHands(player);
    }

    private void SpawnIngredientInPlayersHands(GameObject player)
    {
        GameObject ingredient = Instantiate(ingredientSO.Prefab, spawnTransform.position, Quaternion.identity);
        Grabable grabComponent = ingredient.GetComponent<Grabable>();
        GrabScript grabScript = player.GetComponent<GrabScript>();
        grabScript.MakePlayerGrabObject(grabComponent);
    }
}
