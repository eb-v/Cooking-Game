using UnityEngine;
using System.Collections;

public class Burnable : MonoBehaviour, IFlammable {
    [Header("Fire Spread Settings")]
    public bool canCatchFire = true;
    public float ignitionDelay = 3f;
    public float spreadRadius = 2f;

    public bool IsOnFire => isOnFire;
    public bool CanCatchFire => canCatchFire;

    [HideInInspector] public bool isOnFire = false;

    private Coroutine ignitionRoutine;

    public void Ignite() {
        if (isOnFire || !canCatchFire) return;
        isOnFire = true;

        FireSystem.IgniteObject(gameObject);
        StartCoroutine(SpreadFire());
    }

    public void Extinguish() {
        isOnFire = false;
        FireSystem.ExtinguishObject(gameObject);
    }

    public void TryIgniteWithDelay() {
        if (isOnFire || ignitionRoutine != null || !canCatchFire) return;
        ignitionRoutine = StartCoroutine(IgnitionCountdown());
    }

    private IEnumerator IgnitionCountdown() {
        yield return new WaitForSeconds(ignitionDelay);
        Ignite();
    }

    private IEnumerator SpreadFire() {
        while (isOnFire) {
            Collider[] hits = Physics.OverlapSphere(transform.position, spreadRadius);
            foreach (var hit in hits) {
                if (hit.TryGetComponent(out IFlammable flame) &&
                    flame.CanCatchFire &&
                    !flame.IsOnFire) {
                    flame.Ignite();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, spreadRadius);
    }
}
