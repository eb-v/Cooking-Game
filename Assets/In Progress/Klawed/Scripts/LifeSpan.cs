using UnityEngine;
using System.Collections;

public class LifeSpan : MonoBehaviour
{
    [SerializeField] private float lifeDuration = 5f;

    private void OnEnable()
    {
        DespawnObjectAfterDelay(lifeDuration);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
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
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
