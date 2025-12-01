using System.Collections.Generic;
using UnityEngine;

public class CloseProximityManager : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Players that must stay close. If empty, will find all objects tagged 'Player'.")]
    public List<Transform> players = new List<Transform>();

    [Header("Distance Settings")]
    [Tooltip("Base max distance between a player and their nearest buddy.")]
    public float baseMaxDistance = 3f;

    [Tooltip("Should max distance be randomized each round?")]
    public bool randomizeEachRound = true;

    [Tooltip("Range used when randomizeEachRound is true.")]
    public Vector2 randomDistanceRange = new Vector2(2f, 5f);

    [Tooltip("How long they can be too far apart before punishment.")]
    public float graceTime = 1.5f;

    [Header("FX")]
    [Tooltip("Optional explosion prefab to spawn at players when they fail.")]
    public GameObject explosionPrefab;

    [Header("UI Indicator")]
    [Tooltip("Icon prefab (e.g., a chain) that appears above each player when Close Proximity is active.")]
    public GameObject proximityIconPrefab;

    [Tooltip("Offset from each player's position for the icon.")]
    public Vector3 iconOffset = new Vector3(0f, 2f, 0f);

    private bool _isActive = false;
    private float _currentMaxDistance;
    private float _timer;

    // spawned icon instances, one per player
    private readonly List<GameObject> _spawnedIcons = new List<GameObject>();

    private void Awake()
    {
        // Auto-find players if not assigned
        if (players.Count == 0)
        {
            GameObject[] found = GameObject.FindGameObjectsWithTag("Player");
            foreach (var go in found)
            {
                players.Add(go.transform);
            }
        }
    }

    private void OnEnable()
    {
        GenericEvent<OnModifiersChoosenEvent>
            .GetEvent("OnModifiersChoosenEvent")
            .AddListener(OnModifiersChosen);
    }

    private void OnDisable()
    {
        GenericEvent<OnModifiersChoosenEvent>
            .GetEvent("OnModifiersChoosenEvent")
            .RemoveListener(OnModifiersChosen);

        DestroyIcons();
    }

    private void OnModifiersChosen(List<LevelModifiers> activeModifiers)
    {
        if (activeModifiers.Contains(LevelModifiers.CloseProximity))
        {
            ActivateModifier();
        }
        else
        {
            DeactivateModifier();
        }
    }

    private void ActivateModifier()
    {
        if (players.Count < 2)
        {
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

    private void DeactivateModifier()
    {
        _isActive = false;
        _timer = graceTime;
        Debug.Log("[CloseProximity] Deactivated.");

        DestroyIcons();
    }

    private void Update()
    {
        if (!_isActive) return;
        if (FreezeManager.PauseMenuOverride) return;
        if (players.Count < 2) return;

        float worstNearestDist = 0f;

        for (int i = 0; i < players.Count; i++)
        {
            Transform pi = players[i];
            if (pi == null) continue;

            float nearest = float.MaxValue;

            for (int j = 0; j < players.Count; j++)
            {
                if (i == j) continue;

                Transform pj = players[j];
                if (pj == null) continue;

                float dist = Vector3.Distance(pi.position, pj.position);
                if (dist < nearest)
                {
                    nearest = dist;
                }
            }

            if (nearest > worstNearestDist)
            {
                worstNearestDist = nearest;
            }
        }

        bool shouldWarn = worstNearestDist > 0.85f * _currentMaxDistance;
        SetIconsWarning(shouldWarn);

        // Check distance against allowed max
        if (worstNearestDist > _currentMaxDistance)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                TriggerPunishment();
                _timer = graceTime; // can punish again if they stay apart
            }
        }
        else
        {
            // They're back close together, reset timer
            _timer = graceTime;
        }
    }

    private void TriggerPunishment()
    {
        Debug.Log("[CloseProximity] Players stayed apart too long! BOOM.");

        foreach (Transform p in players)
        {
            if (p == null) continue;

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, p.position, Quaternion.identity);
            }

            // TODO: Hook into your real death/fail logic here
            // var health = p.GetComponent<PlayerHealth>();
            // if (health != null) health.Kill();
        }
    }

    private void CreateIcons()
    {
        DestroyIcons();

        if (proximityIconPrefab == null)
        {
            Debug.LogWarning("[CloseProximity] No proximityIconPrefab assigned, icons will not be shown.");
            return;
        }

        foreach (Transform p in players)
        {
            if (p == null) continue;

            GameObject icon = Instantiate(
                proximityIconPrefab,
                p.position + iconOffset,
                Quaternion.identity
            );

            // Make it follow the player
            icon.transform.SetParent(p, true);

            _spawnedIcons.Add(icon);
        }
    }

    private void DestroyIcons()
    {
        foreach (GameObject icon in _spawnedIcons)
        {
            if (icon != null)
            {
                Destroy(icon);
            }
        }
        _spawnedIcons.Clear();
    }

    private void SetIconsWarning(bool warning)
    {
        foreach (GameObject icon in _spawnedIcons)
        {
            if (icon == null) continue;

            // Try SpriteRenderer first
            var sr = icon.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = warning ? Color.red : Color.white;
                continue;
            }

            // Or a UI Image if you're using a world-space canvas
            var img = icon.GetComponentInChildren<UnityEngine.UI.Image>();
            if (img != null)
            {
                img.color = warning ? Color.red : Color.white;
            }
        }
    }

    // Optional debug buttons in inspector
    [ContextMenu("DEBUG: Activate CloseProximity")]
    private void DebugActivate() => ActivateModifier();

    [ContextMenu("DEBUG: Deactivate CloseProximity")]
    private void DebugDeactivate() => DeactivateModifier();
}
