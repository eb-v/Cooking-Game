using System.Collections.Generic;
using UnityEngine;

public class CloseProximityManager : MonoBehaviour {
    [Header("Player Settings")]
    public List<Transform> players = new List<Transform>();

    [Header("Distance Settings")]
    public float baseMaxDistance = 3f;
    public bool randomizeEachRound = true;
    public Vector2 randomDistanceRange = new Vector2(2f, 5f);
    public float graceTime = 1.5f;

    [Header("FX")]
    public GameObject explosionPrefab;

    [Header("UI Indicator")]
    public GameObject proximityIconPrefab;
    public Vector3 iconOffset = new Vector3(0f, 2f, 0f);

    private bool _isActive = false;
    private float _currentMaxDistance;
    private float _timer;
    private readonly List<GameObject> _spawnedIcons = new List<GameObject>();

    private void Awake() {
        // Auto-find players if not assigned
        if (players.Count == 0) {
            GameObject[] found = GameObject.FindGameObjectsWithTag("Player");
            foreach (var go in found) {
                players.Add(go.transform);
            }
        }
    }

    private void Update() {
        if (!_isActive) return;          // Do nothing if not active
        if (FreezeManager.PauseMenuOverride) return;
        if (players.Count < 2) return;

        float worstNearestDist = 0f;

        for (int i = 0; i < players.Count; i++) {
            Transform pi = players[i];
            if (pi == null) continue;

            float nearest = float.MaxValue;

            for (int j = 0; j < players.Count; j++) {
                if (i == j) continue;

                Transform pj = players[j];
                if (pj == null) continue;

                float dist = Vector3.Distance(pi.position, pj.position);
                if (dist < nearest) nearest = dist;
            }

            if (nearest > worstNearestDist) worstNearestDist = nearest;
        }

        bool shouldWarn = worstNearestDist > 0.85f * _currentMaxDistance;
        SetIconsWarning(shouldWarn);

        if (worstNearestDist > _currentMaxDistance) {
            _timer -= Time.deltaTime;

            if (_timer <= 0f) {
                TriggerPunishment();
                _timer = graceTime; // can punish again if they stay apart
            }
        } else {
            _timer = graceTime; // reset timer if back together
        }
    }

    public void ActivateModifier() {
        if (players.Count < 2) {
            Debug.LogWarning("[CloseProximity] Not enough players to enforce proximity.");
            _isActive = false;
            DestroyIcons();
            return;
        }

        _isActive = true;
        _timer = graceTime;
        _currentMaxDistance = randomizeEachRound
            ? Random.Range(randomDistanceRange.x, randomDistanceRange.y)
            : baseMaxDistance;

        Debug.Log("[CloseProximity] Activated. Max distance = " + _currentMaxDistance);

        CreateIcons();
    }

    public void DeactivateModifier() {
        _isActive = false;
        _timer = graceTime;
        Debug.Log("[CloseProximity] Deactivated.");
        DestroyIcons();
    }

    private void TriggerPunishment() {
        Debug.Log("[CloseProximity] Players stayed apart too long! BOOM.");

        foreach (Transform p in players) {
            if (p == null) continue;
            if (explosionPrefab != null) Instantiate(explosionPrefab, p.position, Quaternion.identity);
        }
    }

    private void CreateIcons() {
        DestroyIcons();

        if (proximityIconPrefab == null) return;

        foreach (Transform p in players) {
            if (p == null) continue;

            GameObject icon = Instantiate(proximityIconPrefab, p.position + iconOffset, Quaternion.identity);
            icon.transform.SetParent(p, true);
            _spawnedIcons.Add(icon);
        }
    }

    private void DestroyIcons() {
        foreach (GameObject icon in _spawnedIcons) {
            if (icon != null) Destroy(icon);
        }
        _spawnedIcons.Clear();
    }

    private void SetIconsWarning(bool warning) {
        foreach (GameObject icon in _spawnedIcons) {
            if (icon == null) continue;

            var sr = icon.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) {
                sr.color = warning ? Color.red : Color.white;
                continue;
            }

            var img = icon.GetComponentInChildren<UnityEngine.UI.Image>();
            if (img != null) img.color = warning ? Color.red : Color.white;
        }
    }
}
