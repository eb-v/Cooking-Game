using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class LightningModifier : MonoBehaviour {
    [Header("Lightning Settings")]
    public GameObject lightningPrefab;
    public Vector3 areaCenter;
    public Vector3 areaSize;
    public float strikeInterval = 10f;
    public int maxStrikes = 20;

    private int strikesSpawned = 0;

    [Header("Floor Detection")]
    public LayerMask floorLayerMask;
    public float raycastHeight = 30f;
    public float raycastDistance = 100f;
    public float spawnYOffset = 0.2f;

    [Header("Audio")]
    public AudioClip lightningSound;
    private AudioSource audioSource;

    private void Awake() {
        if (!TryGetComponent(out audioSource)) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start() {
        TriggerLightning();
    }

    public void TriggerLightning() {
        StartCoroutine(LightningRoutine());
    }

    private void SpawnLightning() {
        if (!TryGetStrikePoint(out Vector3 strikePos))
        {
            Debug.LogWarning("LightningModifier: Couldn't find valid floor hit for lightning.");
            return;
        }

        Quaternion rot = Quaternion.Euler(90f, 0f, 0f);
        Instantiate(lightningPrefab, strikePos, rot);

        if (lightningSound != null && audioSource != null)
            audioSource.PlayOneShot(lightningSound, 0.1f);

        strikesSpawned++;
        Debug.Log("Lightning spawned at " + strikePos);
    }

    private bool TryGetStrikePoint(out Vector3 hitPoint)
    {
        const int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            // random X/Z inside the rect
            float x = Random.Range(areaCenter.x - areaSize.x / 2f,
                                   areaCenter.x + areaSize.x / 2f);
            float z = Random.Range(areaCenter.z - areaSize.z / 2f,
                                   areaCenter.z + areaSize.z / 2f);

            Vector3 rayOrigin = new Vector3(x, areaCenter.y + raycastHeight, z);

            // Only hit colliders on the floorLayerMask
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastDistance, floorLayerMask))
            {
                hitPoint = hit.point + Vector3.up * spawnYOffset;
                return true;
            }
        }

        hitPoint = Vector3.zero;
        return false;
    }

    private IEnumerator LightningRoutine() {
        strikesSpawned = 0;
        while (strikesSpawned < maxStrikes) {
            SpawnLightning();
            yield return new WaitForSeconds(strikeInterval);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}
