using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class FireController : MonoBehaviour {
    [Header("Fire Components")]
    [SerializeField] private ParticleSystem[] fireParticles;

    [Header("Fire Settings")]
    [SerializeField] private float burnDuration = 20f;

    private bool burning = false;

    private void Awake() {
        // Auto-fill particle systems if not assigned
        if (fireParticles == null || fireParticles.Length == 0)
            fireParticles = GetComponentsInChildren<ParticleSystem>();

        ResetFire();

        // Subscribe to global fire events
        GenericEvent<StartFireEvent>.GetEvent("FireController").AddListener(OnStartFireEvent);
        GenericEvent<StopFireEvent>.GetEvent("FireController").AddListener(OnStopFireEvent);
    }

    private void OnDestroy() {
        // Always unsubscribe to prevent leaks
        GenericEvent<StartFireEvent>.GetEvent("FireController").RemoveListener(OnStartFireEvent);
        GenericEvent<StopFireEvent>.GetEvent("FireController").RemoveListener(OnStopFireEvent);
    }

    private void OnStartFireEvent() {
        StartFire();
    }

    private void OnStopFireEvent() {
        StopFire();
    }

    public void StartFire() {
        if (burning) return;
        burning = true;

        foreach (var ps in fireParticles) {
            if (ps != null) {
                ps.Clear();
                ps.Play();
            }
        }

        if (burnDuration > 0)
            StartCoroutine(AutoStopFire());
    }

    private IEnumerator AutoStopFire() {
        yield return new WaitForSeconds(burnDuration);
        GenericEvent<StopFireEvent>.GetEvent("FireController").Invoke();
    }

    public void StopFire() {
        if (!burning) return;
        burning = false;

        foreach (var ps in fireParticles) {
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        StartCoroutine(ReturnFireNextFrame());
    }

    private IEnumerator ReturnFireNextFrame() {
        yield return null;

        if (ObjectPoolManager.IsPooledObject(gameObject)) {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            Debug.Log($"[FireController] Fire returned to pool: {name}");
        } else {
            Destroy(gameObject);
            Debug.LogWarning($"[FireController] Fire was not pooled — destroyed instead: {name}");
        }
    }

    public void StopFireImmediate() {
        if (!burning) return;
        StopFire();
    }

    public void ResetFire() {
        burning = false;

        foreach (var ps in fireParticles) {
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
