using UnityEngine;
using System.Collections.Generic;


public class spawndrinks : MonoBehaviour
{
    [SerializeField] private List<MenuItem> menuItem;
    [SerializeField] private Transform spawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawndrinkss();
    }

    private void spawndrinkss()
    {
        foreach (var drink in menuItem)
        {
            ObjectPoolManager.SpawnObject(drink.GetOrderItemPrefab(), spawnPoint.position, Quaternion.identity);
            ObjectPoolManager.SpawnObject(drink.GetOrderItemPrefab(), spawnPoint.position, Quaternion.identity);
            ObjectPoolManager.SpawnObject(drink.GetOrderItemPrefab(), spawnPoint.position, Quaternion.identity);
            ObjectPoolManager.SpawnObject(drink.GetOrderItemPrefab(), spawnPoint.position, Quaternion.identity);
        }
    }

}
