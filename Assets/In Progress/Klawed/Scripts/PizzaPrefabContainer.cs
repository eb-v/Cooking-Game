using UnityEngine;

public class PizzaPrefabContainer : MonoBehaviour
{
    [SerializeField] private GameObject _cookedPizzaPrefab;

    public GameObject GetCookedPrefab() => _cookedPizzaPrefab;
}
