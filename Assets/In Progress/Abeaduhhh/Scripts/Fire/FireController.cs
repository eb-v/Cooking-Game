using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour
{
    [Header("Fire Components")]
    [SerializeField] private ParticleSystem[] fireParticles;

    [Header("Fire Settings")]
    [SerializeField] private float burnDuration = 60f;

    private bool burning = false;

    private void Awake()
    {
        if (fireParticles == null || fireParticles.Length == 0)
            fireParticles = GetComponentsInChildren<ParticleSystem>();

        ResetFire();

        GenericEvent<StartFireEvent>.GetEvent("FireController").AddListener(OnStartFireEvent);
        GenericEvent<StopFireEvent>.GetEvent("FireController").AddListener(OnStopFireEvent);
    }

    private void OnDestroy()
    {
        GenericEvent<StartFireEvent>.GetEvent("FireController").RemoveListener(OnStartFireEvent);
        GenericEvent<StopFireEvent>.GetEvent("FireController").RemoveListener(OnStopFireEvent);

        if (burning)
        {
            burning = false;
            FireAudioManager.Instance?.UnregisterFire();
        }
    }

    private void OnStartFireEvent() => StartFire();
    private void OnStopFireEvent()  => StopFire();

    public void StartFire()
    {
        if (burning) return;
        burning = true;

        foreach (var ps in fireParticles)
        {
            if (ps != null)
            {
                ps.Clear();
                ps.Play();
            }
        }

        FireAudioManager.Instance?.RegisterFire();

        if (burnDuration > 0)
            StartCoroutine(AutoStopFire());
    }

    private IEnumerator AutoStopFire()
    {
        yield return new WaitForSeconds(burnDuration);
        GenericEvent<StopFireEvent>.GetEvent("FireController").Invoke();
    }

    public void StopFire()
    {
        if (!burning) return;
        burning = false;

        foreach (var ps in fireParticles)
        {
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        FireAudioManager.Instance?.UnregisterFire();
    }

    public void StopFireImmediate()
    {
        if (!burning) return;

        burning = false;

        foreach (var ps in fireParticles)
        {
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        FireAudioManager.Instance?.UnregisterFire();
    }

    public void ResetFire()
    {
        burning = false;

        foreach (var ps in fireParticles)
        {
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
