using UnityEngine;

public class SpawnIngredient : MonoBehaviour
{
    [SerializeField] private GameObject _ingredientPrefab;
    [SerializeField] private string _eventChannel;
    public Transform playerTransform;
    public Vector3 playerPos;
    public float spawnHeight = 3f;

    private void Awake()
    {
        GenericEvent<OnSpawnIngredient>.GetEvent(_eventChannel).AddListener(SpawnSelectedIngredient);

    }


    private void SpawnSelectedIngredient(Vector3 spawn)
    {
        spawn += playerTransform.position;
        spawn.y = spawnHeight;

        ObjectPoolManager.SpawnObject(_ingredientPrefab, spawn, Quaternion.identity);
        Debug.Log("Spawned Ingredient at " + spawn);
    }
    

}
