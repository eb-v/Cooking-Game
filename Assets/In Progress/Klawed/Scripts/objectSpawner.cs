using UnityEngine;

public class objectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;


    public void SpawnObject()
    {
        ObjectPoolManager.SpawnObject(objectToSpawn, transform.position, Quaternion.identity);
    }
}
