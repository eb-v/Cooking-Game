using UnityEngine;

public class spawnstuff : MonoBehaviour
{
    [SerializeField] private GameObject topbunprefab;
    [SerializeField] private GameObject bottombunprefab;
    [SerializeField] private GameObject meatprefab;
    [SerializeField] private GameObject cheeseprefab;
    [SerializeField] private GameObject tomatoprefab;
    [SerializeField] private GameObject lettuceprefab;
    [SerializeField] private Transform spawnpoint;

    void Start()
    {
        SpawnIngredient(topbunprefab);
        SpawnIngredient(bottombunprefab);
        SpawnIngredient(meatprefab);
        SpawnIngredient(cheeseprefab);
        SpawnIngredient(tomatoprefab);
        SpawnIngredient(lettuceprefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnIngredient(GameObject ingredientPrefab)
    {
        ObjectPoolManager.SpawnObject(ingredientPrefab, spawnpoint.position, Quaternion.identity);
    }
}
