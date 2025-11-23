using UnityEngine;

[CreateAssetMenu(fileName = "BurnableSettings", menuName = "Status Effects/BurnableSettings")]
public class BurnableSettings : ScriptableObject {
    //[Header("Burn Settings")]
    //public float burnThreshold = 1f;
    //[Tooltip("Base burn speed multiplier applied to burn progress.")]
    //public float burnSpeedMultiplier = 1f;

    [Header("Spread Control")]
    public bool allowSpread = true;
    [Tooltip("Multiplier applied to spread intensity.")]
    public float spreadMultiplier = 1f;

    [Header("Optional Spread Amount")]
    public float spreadAmount = 0.2f;
}
