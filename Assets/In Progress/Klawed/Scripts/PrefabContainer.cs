using UnityEngine;

public class PrefabContainer : MonoBehaviour
{
    [SerializeField] private string prefabName;

    public string GetPrefabName() => prefabName;
}
