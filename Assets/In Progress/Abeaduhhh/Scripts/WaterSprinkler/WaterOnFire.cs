using UnityEngine;

public class SprinklerWater : MonoBehaviour {
    [Tooltip("How often this water can trigger fire extinguishing, to prevent spamming.")]
    public float extinguishCooldown = 0.5f;

    private float lastExtinguishTime;

    private void OnTriggerStay(Collider other) {
        if (Time.time < lastExtinguishTime + extinguishCooldown)
            return;

        FireController fire = other.GetComponentInParent<FireController>();
        if (fire != null) {
            fire.StopFireImmediate();
            Debug.Log($"[SprinklerWater] Extinguished fire: {fire.name}");
            lastExtinguishTime = Time.time;
        }
    }
}