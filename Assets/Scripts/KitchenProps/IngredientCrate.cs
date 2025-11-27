using UnityEngine;

public class IngredientCrate : MonoBehaviour, IInteractable
{
    [SerializeField] private Ingredient ingredientSO;
    [SerializeField] private Transform spawnTransform;

    public void OnInteract(GameObject player)
    {
        Debug.Log("Player interacted with Ingredient Crate to spawn: " + ingredientSO.Prefab.name);
        Transform playerRoot = player.GetComponent<RagdollController>().GetPelvis().transform;
        Vector3 direction = (spawnTransform.position - playerRoot.position).normalized;

        SpawnIngredientInPlayersHands(player);
    }

    private void SpawnIngredientInPlayersHands(GameObject player)
    {
        GameObject ingredient = Instantiate(ingredientSO.Prefab, spawnTransform.position, Quaternion.identity);
        IGrabable grabComponent = ingredient.GetComponent<IGrabable>();
        grabComponent.GrabObject(player);
    }
}
