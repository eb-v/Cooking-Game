using UnityEngine;
using System.Collections;

public class LifeSpan : MonoBehaviour
{
    [SerializeField] private float lifeDuration = 5f;

    private void Start()
    {
        DespawnObjectAfterDelay(lifeDuration);
    }

    private void DespawnObjectAfterDelay(float delay)
    {
        StartCoroutine(Despawner(delay));
    }

    private IEnumerator Despawner(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ObjectPoolManager.IsPooledObject(gameObject))
        {
            Rigidbody rb = gameObject.GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
