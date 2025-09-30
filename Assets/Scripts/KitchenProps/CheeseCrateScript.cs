using UnityEngine;

public class CheeseCrateScript : MonoBehaviour
{
    [SerializeField] private GameObject cheesePrefab;
    [SerializeField] private Transform spawnPoint;


    private void Awake()
    {
        GenericEvent<Interact>.GetEvent(gameObject.name).AddListener(SpawnIngredient);
    }

    private void SpawnIngredient()
    {
        ObjectPoolManager.SpawnObject(cheesePrefab, spawnPoint, Quaternion.identity);
    }
}
