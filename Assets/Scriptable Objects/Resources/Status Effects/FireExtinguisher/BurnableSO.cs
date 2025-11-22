using UnityEngine;

[CreateAssetMenu(fileName = "BurnableSettings", menuName = "Status Effects/BurnableSettings")]
public class BurnableSettings : ScriptableObject {
    [Header("Fire Spread Settings")]
    public bool canCatchFire = true;
    public float spreadRadius = 2f;

    [Header("Burn Settings")]
    public bool autoIgnite = true;
    public float burnThreshold = 1f;
    [Tooltip("Base burn speed multiplier applied to burn progress.")]
    public float burnSpeedMultiplier = 1f;

    [Header("Fire Control")]
    public bool useIgnitionDelay = true;

    [Header("Spread Control")]
    public bool allowSpread = true;
    [Tooltip("Multiplier applied to spread intensity.")]
    public float spreadMultiplier = 1f;

    [Header("Optional Spread Amount")]
    public float spreadAmount = 0.2f;
}
