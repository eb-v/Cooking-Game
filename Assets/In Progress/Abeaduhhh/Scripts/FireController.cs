using UnityEngine;

public class FireController : MonoBehaviour {
    [Header("Fire Components")]
    public ParticleSystem[] fireParticles;

    [Header("Fire Settings")]
    public float burnDuration = 10f;
    public bool autoDestroy = false;

    private bool burning = false;

    void Awake() {
        if (fireParticles == null || fireParticles.Length == 0)
            fireParticles = GetComponentsInChildren<ParticleSystem>();
        ResetFire();
    }

    public void StartFire() {
        if (burning) return;
        burning = true;

        foreach (var ps in fireParticles)
            if (ps != null) { ps.Clear(); ps.Play(); }

        if (burnDuration > 0)
            Invoke(nameof(StopFire), burnDuration);
    }

    public void StopFire() {
        if (!burning) return;
        burning = false;

        foreach (var ps in fireParticles) {
            if (ps != null) {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        CancelInvoke(nameof(StopFire));

        if (autoDestroy)
            ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    public void StopFireImmediate() {
        if (!burning) return;
        CancelInvoke(nameof(StopFire));
        StopFire();
    }

    public void ResetFire() {
        burning = false;
        foreach (var ps in fireParticles)
            if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        CancelInvoke(nameof(StopFire));
    }
}
