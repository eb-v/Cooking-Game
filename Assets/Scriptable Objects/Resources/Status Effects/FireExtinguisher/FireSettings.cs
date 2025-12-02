using UnityEngine;

[CreateAssetMenu(fileName = "BurnableSettings", menuName = "Status Effects/BurnableSettings")]
public class FireSettings : ScriptableObject {
    //[Header("Burn Settings")]
    //public float burnThreshold = 1f;
    //[Tooltip("Base burn speed multiplier applied to burn progress.")]
    //public float burnSpeedMultiplier = 1f;

    [Tooltip("Max Time an object can stay on fire.")]
    public float maxBurnTime = 10f;
    public float burnProgressLockDuration = 2f;
    public float burnMultiplier = 0.4f;
    public float extinguishRate = 0.3f;
    public float ignitionCooldown = 3f;
    public int maxSimultaneousFires = 15;

}
