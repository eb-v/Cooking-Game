using System.Collections;
using UnityEngine;

public class BombDropper : MonoBehaviour
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;

    [Header("Drop Area (X/Z bounds)")]
    public float minX = -10f;
    public float maxX =  10f;
    public float minZ = -10f;
    public float maxZ =  10f;

    [Header("Drop Height & Timing")]
    public float dropHeight = 20f;
    public float timeBetweenDrops = 1.0f;

    [Header("Floor Tag")]
    public string floorTag = "Floor";
    public string groundTag = "Ground";

    [Header("Start/Stop")]
    public bool autoStart = false; 

    private Coroutine dropRoutine;

    void Start()
    {
        if (autoStart)
        {
            StartDropping();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 p1 = new Vector3(minX, 0f, minZ);
        Vector3 p2 = new Vector3(minX, 0f, maxZ);
        Vector3 p3 = new Vector3(maxX, 0f, maxZ);
        Vector3 p4 = new Vector3(maxX, 0f, minZ);

        Gizmos.DrawLine(p1, p2);
        // Line 2: p2 -> p3 (Top side)
        Gizmos.DrawLine(p2, p3);
        // Line 3: p3 -> p4 (Right side)
        Gizmos.DrawLine(p3, p4);
        // Line 4: p4 -> p1 (Bottom side)
        Gizmos.DrawLine(p4, p1);

        Vector3 center = new Vector3((minX + maxX) / 2f, 0f, (minZ + maxZ) / 2f);
        Gizmos.DrawWireSphere(center, 0.5f);
    }

    public void StartDropping()
    {
        Debug.Log("Bomb system activated!");
        if (dropRoutine == null)
        {
            dropRoutine = StartCoroutine(DropBombsLoop());
        }
    }

    public void StopDropping()
    {
        if (dropRoutine != null)
        {
            StopCoroutine(dropRoutine);
            dropRoutine = null;
        }
    }

    private IEnumerator DropBombsLoop()
    {
        Debug.Log("BombDropper: Starting bomb drop loop...");
        while (true)
        {
            Debug.Log("BombDropper: Attempting to spawn bomb...");
            TrySpawnBombAboveFloor();
            yield return new WaitForSeconds(timeBetweenDrops);
        }
    }

    private void TrySpawnBombAboveFloor()
    {
        if (bombPrefab == null)
        {
            Debug.LogWarning("BombDropper: No bombPrefab assigned!");
            return;
        }

        for (int i = 0; i < 30; i++)
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);

            Vector3 rayOrigin = new Vector3(x, dropHeight + 10f, z);
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f))
            {
                if (hit.collider.CompareTag(floorTag) || hit.collider.CompareTag(groundTag))
                {
                    Vector3 spawnPos = new Vector3(x, dropHeight, z);
                    Instantiate(bombPrefab, spawnPos, Quaternion.identity);
                    return;
                }
            }
        }
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        // Center of the drop area
        Vector3 center = new Vector3(
            (minX + maxX) / 2f,
            dropHeight,
            (minZ + maxZ) / 2f
        );

        // Size of the drop area
        Vector3 size = new Vector3(
            Mathf.Abs(maxX - minX),
            0.1f,
            Mathf.Abs(maxZ - minZ)
        );

        // Draw the drop area at the drop height
        Gizmos.DrawWireCube(center, size);

        // Optional: draw a filled transparent box
        Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
        Gizmos.DrawCube(center, size);
    }
}
