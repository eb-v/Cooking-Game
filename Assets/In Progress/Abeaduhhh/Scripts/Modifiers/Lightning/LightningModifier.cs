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

    [Header("Audio")]
    public AudioClip lightningSound;
    private AudioSource audioSource;

    private void Awake() {
        if (!TryGetComponent(out audioSource)) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    //private void Start() {
    //    TriggerLightning();
    //}

    public void TriggerLightning() {
        StartCoroutine(LightningRoutine());
    }

    private void SpawnLightning() {
        Vector3 spawnPos = GetRandomPointInRect();
        Quaternion rot = Quaternion.Euler(90f, 0f, 0f);
        Instantiate(lightningPrefab, spawnPos, rot);

        if (lightningSound != null && audioSource != null)
            audioSource.PlayOneShot(lightningSound, 0.1f);

        strikesSpawned++;
        Debug.Log("Lightning spawned");
    }

    private Vector3 GetRandomPointInRect() {
        float x = Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f);
        float y = areaCenter.y + areaSize.y;
        float z = Random.Range(areaCenter.z - areaSize.z / 2f, areaCenter.z + areaSize.z / 2f);

        return new Vector3(x, y, z);
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
