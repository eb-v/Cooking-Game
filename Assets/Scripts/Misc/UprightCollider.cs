using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UprightCollider : MonoBehaviour
{
    [ReadOnly]
    [SerializeField] private List<Collider> colliders = new List<Collider>();

    private void Start()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            colliders.Add(col);
        }
    }

    private void LateUpdate()
    {
        foreach (Collider col in colliders)
        {
            col.transform.rotation = Quaternion.identity;
        }
    }
}
