using UnityEngine;

public class PrefabContainer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    public string GetPrefabName() => prefab.name;
}
