using UnityEngine;

public class EarthquakeKnockOff : MonoBehaviour {
    public float radius = 5f;
    public float force = 50f;

    public LayerMask affectedLayers;

    public void TriggerEarthquake() {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, affectedLayers);

        foreach (Collider col in cols) {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null) {
                Vector3 dir = Random.insideUnitSphere.normalized;
                rb.AddForce(dir * force, ForceMode.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
