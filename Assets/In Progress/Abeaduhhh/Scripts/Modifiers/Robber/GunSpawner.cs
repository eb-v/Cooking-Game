using UnityEngine;

public class GunSpawner : MonoBehaviour {
    [Header("Gun Settings")]
    [SerializeField] private GameObject gunPrefab;
    [SerializeField] private Transform gunSpawnPoint;

    private GameObject heldGun;
    public void SpawnGun() {
        if (gunPrefab == null) {
            Debug.LogWarning($"{name}: Gun prefab not assigned!");
            return;
        }

        if (gunSpawnPoint == null) {
            Debug.LogWarning($"{name}: Gun spawn point not assigned!");
            return;
        }

        if (heldGun != null) {
            Destroy(heldGun);
        }

        heldGun = Instantiate(gunPrefab, gunSpawnPoint.position, gunSpawnPoint.rotation, gunSpawnPoint);

        heldGun.transform.localScale = Vector3.one;

        Rigidbody rb = heldGun.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Debug.Log($"{name} spawned {gunPrefab.name} at {gunSpawnPoint.name}");
    }

    public void PointGun() {
        SpawnGun();
    }
}
