using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("The oil hazard prefab to spawn")]
    public GameObject oilPrefab;

    [Tooltip("List of spawn points where oil can appear")]
    public Transform[] spawnPoints;

    [Header("Timing Settings")]
    [Tooltip("Time before first oil spawns")]
    public float initialDelay = 5f;

    [Tooltip("Minimum time between spawns")]
    public float minSpawnInterval = 3f;

    [Tooltip("Maximum time between spawns")]
    public float maxSpawnInterval = 8f;

    [Header("Spawn Behavior")]
    [Tooltip("Maximum number of oil hazards active at once (0 = unlimited)")]
    public int maxActiveOils = 3;

    [Tooltip("How long before an oil hazard despawns (0 = never)")]
    public float despawnTime = 15f;

    [Tooltip("Should spawning start automatically on scene load?")]
    public bool autoStart = true;

    private List<GameObject> activeOils = new List<GameObject>();
    private bool isSpawning = false;

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    private void OnEnable()
    {
        StartSpawning();
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        // Wait for initial delay
        yield return new WaitForSeconds(initialDelay);

        while (isSpawning)
        {
            // Check if we can spawn more oils
            if (maxActiveOils == 0 || activeOils.Count < maxActiveOils)
            {
                SpawnOil();
            }

            // Wait for random interval before next spawn
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void SpawnOil()
    {
        // Validate spawn settings
        if (oilPrefab == null)
        {
            Debug.LogWarning("OilSpawnManager: No oil prefab assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("OilSpawnManager: No spawn points assigned!");
            return;
        }

        // Remove null entries from active oils list
        activeOils.RemoveAll(oil => oil == null);

        // Pick a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Spawn the oil hazard
        GameObject newOil = Instantiate(oilPrefab, spawnPoint.position, spawnPoint.rotation);
        activeOils.Add(newOil);

        // Set up despawn if enabled
        if (despawnTime > 0)
        {
            StartCoroutine(DespawnAfterTime(newOil, despawnTime));
        }

        Debug.Log($"Oil spawned at {spawnPoint.name}. Active oils: {activeOils.Count}");
    }

    private IEnumerator DespawnAfterTime(GameObject oil, float time)
    {
        yield return new WaitForSeconds(time);

        if (oil != null)
        {
            activeOils.Remove(oil);
            Destroy(oil);
        }
    }

    public void ClearAllOils()
    {
        foreach (GameObject oil in activeOils)
        {
            if (oil != null)
            {
                Destroy(oil);
            }
        }
        activeOils.Clear();
    }

    private void OnDrawGizmos()
    {
        // Visualize spawn points in the editor
        if (spawnPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                    Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 2f);
                }
            }
        }
    }
}
