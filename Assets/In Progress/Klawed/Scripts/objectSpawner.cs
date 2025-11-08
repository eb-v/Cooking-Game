using UnityEngine;
using System.Collections.Generic;

public class objectSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToSpawn;


    public void SpawnObject()
    {
        foreach (GameObject obj in objectsToSpawn)
        {
            ObjectPoolManager.SpawnObject(obj, transform.position, Quaternion.identity);
        }
    }
}
