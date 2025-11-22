using UnityEngine;
using System.Collections;

public class Burnable : MonoBehaviour, IFlammable {
    [Header("Settings Asset")]
    public BurnableSettings settings;

    [Header("Instance Settings")]
    [Tooltip("Multiplier applied to burn speed and spread, editable per object.")]
    public float burnMultiplier = 1f;

    [HideInInspector] public float burnProgress = 0f;
    [HideInInspector] public bool isOnFire = false;

    public bool IsOnFire => isOnFire;
    public bool CanCatchFire => settings != null && settings.canCatchFire;

    public void Ignite() {
        if (isOnFire || !CanCatchFire) return;
        isOnFire = true;

        FireSystem.IgniteObject(gameObject);
        StartCoroutine(SpreadFire());
    }

    public void Extinguish() {
        isOnFire = false;
        burnProgress = 0f;
        FireSystem.ExtinguishObject(gameObject);
    }
    private IEnumerator SpreadFire() {
        if (!settings.allowSpread) yield break;

        while (isOnFire) {
            Collider[] hits = Physics.OverlapSphere(transform.position, settings.spreadRadius);

            foreach (var hit in hits) {
                if (hit.TryGetComponent(out Burnable flame)) {
                    if (flame.CanCatchFire && !flame.IsOnFire) {
                        float appliedSpread = settings.spreadAmount * settings.spreadMultiplier * burnMultiplier * Time.deltaTime;
                        flame.AddBurnProgress(appliedSpread);
                    }
                }
            }
            yield return null;
        }
    }


    public void AddBurnProgress(float amount) {
        if (isOnFire || !settings.autoIgnite) return;

        burnProgress += amount * settings.burnSpeedMultiplier * burnMultiplier;
        burnProgress = Mathf.Clamp01(burnProgress);

        if (burnProgress >= settings.burnThreshold) {
            Ignite();
        }
    }
    public void ReduceBurn(float amount) {
        burnProgress -= amount * settings.burnSpeedMultiplier * burnMultiplier;
        burnProgress = Mathf.Clamp(burnProgress, 0f, 1f);

        if (burnProgress <= 0f && isOnFire) {
            isOnFire = false;
            burnProgress = 0f;
            FireSystem.ExtinguishObject(gameObject);
        }
    }
    private void OnDrawGizmosSelected() {
        if (settings != null) {
            Gizmos.color = new Color(1f, 0.3f, 0f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, settings.spreadRadius);
        }
    }
}
