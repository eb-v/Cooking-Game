using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilSpawnManager : MonoBehaviour {
    [Header("Oil Settings")]
    public GameObject oilPrefab;

    [Header("Spawn Area (XZ bounds)")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minZ = -10f;
    public float maxZ = 10f;

    [Header("Spawn Height")]
    public float spawnY = 0f;

    [Header("Timing Settings")]
    public float initialDelay = 5f;
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 8f;

    [Header("Spawn Behavior")]
    public int maxActiveOils = 3;
    public float despawnTime = 15f;

    private List<GameObject> activeOils = new List<GameObject>();
    private bool isSpawning = false;

    private void OnEnable() {
    }

    private void OnDisable() {
        StopSpawning();
    }

    public void StartSpawning() {
        if (!isSpawning) {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning() {
        isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine() {
        yield return new WaitForSeconds(initialDelay);

        while (isSpawning) {
            activeOils.RemoveAll(o => o == null);

            if (maxActiveOils == 0 || activeOils.Count < maxActiveOils)
                SpawnOil();

            float delay = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(delay);
        }
    }

    public void SpawnOil() {
        if (oilPrefab == null) {
            Debug.LogWarning("OilSpawnManager: No oilPrefab assigned!");
            return;
        }

        Vector3 spawnPos = new Vector3(
            Random.Range(minX, maxX),
            spawnY,
            Random.Range(minZ, maxZ)
        );

        GameObject newOil = Instantiate(oilPrefab, spawnPos, Quaternion.identity);
        activeOils.Add(newOil);

        if (despawnTime > 0)
            StartCoroutine(DespawnAfterTime(newOil, despawnTime));
    }

    private IEnumerator DespawnAfterTime(GameObject oil, float time) {
        yield return new WaitForSeconds(time);
        if (oil != null) {
            activeOils.Remove(oil);
            Destroy(oil);
        }
    }

    public void ClearAllOils() {
        foreach (var oil in activeOils)
            if (oil != null) Destroy(oil);
        activeOils.Clear();
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 0.7f, 0f, 0.25f);
        Gizmos.DrawCube(
            new Vector3((minX + maxX) / 2f, spawnY, (minZ + maxZ) / 2f),
            new Vector3(Mathf.Abs(maxX - minX), 0.1f, Mathf.Abs(maxZ - minZ))
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3((minX + maxX) / 2f, spawnY, (minZ + maxZ) / 2f),
            new Vector3(Mathf.Abs(maxX - minX), 0.1f, Mathf.Abs(maxZ - minZ))
        );
    }
}
