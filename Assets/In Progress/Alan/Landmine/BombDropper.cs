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

    public void StartDropping()
    {
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
        while (true)
        {
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

        for (int i = 0; i < 10; i++)
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);

            Vector3 rayOrigin = new Vector3(x, dropHeight + 10f, z);
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f))
            {
                if (hit.collider.CompareTag(floorTag))
                {
                    Vector3 spawnPos = new Vector3(x, dropHeight, z);
                    Instantiate(bombPrefab, spawnPos, Quaternion.identity);
                    return;
                }
            }
        }
    }
}
