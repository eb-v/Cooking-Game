using UnityEngine;

public class IngredientCrate : MonoBehaviour
{
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private Transform spawnTransform;
    private Vector3 spawnPos;


    private void Awake()
    {
        spawnPos = spawnTransform.position;
        GenericEvent<Interact>.GetEvent(gameObject.name).AddListener(SpawnIngredient);
    }

    private void SpawnIngredient()
    {
        GameObject ingredient = ObjectPoolManager.SpawnObject(ingredientPrefab, spawnPos, Quaternion.identity);
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        rb.isKinematic = false; 

        rb.AddForce(Vector3.up * 8f, ForceMode.Impulse);
    }
}
