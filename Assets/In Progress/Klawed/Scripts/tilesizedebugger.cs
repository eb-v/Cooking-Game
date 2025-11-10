using UnityEngine;

public class tilesizedebugger : MonoBehaviour
{
    void Start()
    {
        var renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            Vector3 worldSize = renderer.bounds.size;
           // Debug.Log($"{name} world size: {worldSize}");
        }
    }
}
