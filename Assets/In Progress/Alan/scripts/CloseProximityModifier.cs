using System.Collections.Generic;
using UnityEngine;

public class CloseProximityManager : MonoBehaviour
{
    [Header("Player Settings")]
    public List<Transform> players = new List<Transform>();
    private List<Transform> playerTransforms = new List<Transform>();


    [Header("Distance Settings")]
    public float baseMaxDistance = 3f;
    public bool randomizeEachRound = true;
    public Vector2 randomDistanceRange = new Vector2(2f, 5f);
    public float graceTime = 15f;

    [Header("Cooldown")]
    [Tooltip("Time between limb pops, even if players stay too far apart.")]
    public float popCooldown = 3f;

    [Header("FX")]
    public GameObject explosionPrefab;

    [Header("UI Indicator")]
    public GameObject proximityIconPrefab;
    public Vector3 iconOffset = new Vector3(0f, 0.35f, 0f);
    [Range(0.05f, 1.0f)]
    public float iconScale = 0.05f;
    private bool _isActive = false;
    private float _currentMaxDistance;
    private float _separationTimer;
    private float _cooldownTimer;
    private readonly List<GameObject> _spawnedIcons = new List<GameObject>();

    [Header("Icon Warning Settings")]
    public float dangerPulseSpeed = 6f;
    public float dangerPulseAmplitude = 0.25f;

private void Awake()
{
    // Make sure lists exist
    if (players == null)
        players = new List<Transform>();

    if (playerTransforms == null)
        playerTransforms = new List<Transform>();

    // Only auto-find if nothing is wired in the inspector
    if (players.Count == 0)
    {
        players.Clear();
        playerTransforms.Clear();

        // Find only root objects that actually have a RagdollController
        RagdollController[] ragdolls =
            Object.FindObjectsByType<RagdollController>(FindObjectsSortMode.None);

        foreach (var rc in ragdolls)
        {
            // Default to the ragdoll root
            Transform tracked = rc.transform;

            // 🔴 Use the pelvis from your RagdollController
            GameObject pelvisGO = rc.GetPelvis();   // public GameObject GetPelvis() => RagdollDict[ROOT].gameObject;

            if (pelvisGO != null)
            {
                tracked = pelvisGO.transform;
            }
            else
            {
                Debug.LogWarning($"[CloseProximity] GetPelvis() returned null on {rc.name}, using root transform.");
            }

            // Fill both lists so old code (icons) and new code (proximity) see the same transforms
            players.Add(tracked);
            playerTransforms.Add(tracked);

            Debug.Log($"[CloseProximity] Registered player: {rc.gameObject.name}, tracking: {tracked.name}");
        }
    }
    else
    {
        // If players were assigned via inspector, mirror them into playerTransforms
        playerTransforms.Clear();
        foreach (var t in players)
        {
            if (t != null)
                playerTransforms.Add(t);
        }

        Debug.Log($"[CloseProximity] Using {players.Count} players from inspector.");
    }
}


    private void Update()
{
    if (!_isActive) return;
    if (FreezeManager.PauseMenuOverride) return;
    if (players.Count < 2) return;

    // tick cooldown
    if (_cooldownTimer > 0f)
        _cooldownTimer -= Time.deltaTime;

    float worstNearestDist = 0f;

    // compute "worst" nearest neighbour distance for any player
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
            if (dist < nearest) nearest = dist;
        }

        if (nearest > worstNearestDist) worstNearestDist = nearest;
    }

    bool tooFar = worstNearestDist > _currentMaxDistance;

    // Normalized distance 0..1 (how close they are to the max radius)
    float distanceRatio = (_currentMaxDistance > 0f)
        ? worstNearestDist / _currentMaxDistance
        : 0f;

    // --- separation / punishment logic ---
    if (tooFar)
    {
        _separationTimer += Time.deltaTime;

        Debug.Log(
            $"[CloseProximity] TOO FAR | worstNearestDist={worstNearestDist:F2}, " +
            $"max={_currentMaxDistance:F2}, sepTimer={_separationTimer:F2}, " +
            $"grace={graceTime:F2}, cooldown={_cooldownTimer:F2}"
        );

        if (_separationTimer >= graceTime && _cooldownTimer <= 0f)
        {
            Debug.Log("[CloseProximity] Grace expired AND cooldown ready → TriggerPunishment()");
            TriggerPunishment();
            _separationTimer = 0f;
            _cooldownTimer = popCooldown;
        }
    }
    else
    {
        if (_separationTimer > 0f)
        {
            Debug.Log(
                $"[CloseProximity] BACK IN RANGE → resetting separation timer (was {_separationTimer:F2}). " +
                $"dist={worstNearestDist:F2}, max={_currentMaxDistance:F2}"
            );
        }

        _separationTimer = 0f;
    }

    // how far through the grace window we are (0..1) when too far
    float separationProgress = 0f;
    if (tooFar && graceTime > 0f)
        separationProgress = Mathf.Clamp01(_separationTimer / graceTime);

    // TEMP: log distance between first two players
if (players.Count >= 2 && players[0] != null && players[1] != null)
{
    float d01 = Vector3.Distance(players[0].position, players[1].position);
    Debug.Log($"[CloseProximity] P0–P1 distance = {d01:F2}");
}

    UpdateIconVisuals(distanceRatio, tooFar, separationProgress);
}
    public void ActivateModifier()
{
    if (players.Count < 2)
    {
        Debug.LogWarning("[CloseProximity] Not enough players to enforce proximity.");
        _isActive = false;
        DestroyIcons();
        return;
    }

    _isActive = true;
    _separationTimer = 0f;
    _cooldownTimer = 0f;

    _currentMaxDistance = randomizeEachRound
        ? Random.Range(randomDistanceRange.x, randomDistanceRange.y)
        : baseMaxDistance;

    Debug.Log(
        $"[CloseProximity] Activated. Max distance = {_currentMaxDistance:F2}, " +
        $"players={players.Count}"
    );

    CreateIcons();
}


    public void DeactivateModifier()
    {
        _isActive = false;
        _separationTimer = 0f;
        _cooldownTimer = 0f;
        Debug.Log("[CloseProximity] Deactivated.");
        DestroyIcons();
    }

    private void TriggerPunishment()
{
    Debug.Log("[CloseProximity] Players stayed apart too long! Trying to pop limbs.");

    foreach (Transform p in players)
    {
        if (p == null) continue;

        // find all LimbHPs under this player
        LimbHP[] limbs = p.GetComponentsInChildren<LimbHP>(true);
        var candidates = new List<LimbHP>();

        foreach (var limb in limbs)
        {
            if (limb == null) continue;

            // only limbs marked as poppable, and not already broken
            if (limb.canBePoppedByProximity && !limb.IsBroken)
                candidates.Add(limb);
        }

        if (candidates.Count == 0)
        {
            Debug.Log("[CloseProximity] No more limbs to pop on " + p.name);
            continue;
        }

        // choose a random limb and nuke it
        LimbHP chosen = candidates[Random.Range(0, candidates.Count)];
        Debug.Log($"[CloseProximity] Popping limb {chosen.gameObject.name} on {p.name}");

        // this should actually break the limb
        chosen.InflictMaxDamage();

        // FX ONLY if we actually popped something
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, chosen.transform.position, Quaternion.identity);
        }
    }
}

    private void CreateIcons()
    {
        DestroyIcons();

        if (proximityIconPrefab == null) return;

        foreach (Transform p in players)
        {
            if (p == null) continue;

            // Find the head if possible, otherwise fall back to root
            Transform attach = FindHeadTransform(p);

            GameObject icon = Instantiate(proximityIconPrefab, attach);
            icon.transform.localPosition = iconOffset;      // local offset above head
            icon.transform.localRotation = Quaternion.identity;
            icon.transform.localScale = Vector3.one * iconScale;

            _spawnedIcons.Add(icon);
        }
    }

    private Transform FindHeadTransform(Transform root)
    {
        // 1) Try to find a RagdollJoint whose name is "Head"
        var joints = root.GetComponentsInChildren<RagdollJoint>(true);
        foreach (var j in joints)
        {
            if (j.GetJointName() == RagdollController.HEAD)
                return j.transform;
        }

        // 2) Fallback by transform path (matches DEF_Head hierarchy)
        var head = root.Find("Armature/DEF_Pelvis/DEF_Body/DEF_Head");
        if (head != null) return head;

        // 3) Absolute fallback: just use the root
        return root;
    }

    private void DestroyIcons()
    {
        foreach (GameObject icon in _spawnedIcons)
        {
            if (icon != null) Destroy(icon);
        }
        _spawnedIcons.Clear();
    }

    private void UpdateIconVisuals(float distanceRatio, bool tooFar, float separationProgress)
{
    // Priority: cooldown > danger timer > distance warnings > normal
    Color c = Color.white;
    float scaleMult = 1f;

    if (_cooldownTimer > 0f)
    {
        // recently popped – show blue and slightly smaller
        c = Color.cyan;
        scaleMult = 0.9f;
    }
    else if (tooFar)
    {
        // Over the max distance and grace timer is running.
        // separationProgress 0..1: just crossed -> almost popping
        c = Color.Lerp(Color.yellow, Color.red, separationProgress);

        // pulse stronger as we get closer to the end of grace time
        float pulse = 1f + dangerPulseAmplitude * separationProgress *
                      Mathf.Sin(Time.time * dangerPulseSpeed);
        scaleMult = pulse;
    }
    else if (distanceRatio >= 0.9f)
    {
        // very close to the max distance but not yet over
        c = new Color(1f, 0.5f, 0f); // orange
        scaleMult = 1.1f;
    }
    else if (distanceRatio >= 0.7f)
    {
        // getting close: soft warning
        c = Color.yellow;
        scaleMult = 1.0f;
    }

    foreach (GameObject icon in _spawnedIcons)
    {
        if (icon == null) continue;

        var sr = icon.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = c;

        var img = icon.GetComponentInChildren<UnityEngine.UI.Image>();
        if (img != null) img.color = c;

        icon.transform.localScale = Vector3.one * iconScale * scaleMult;
    }
}

}
