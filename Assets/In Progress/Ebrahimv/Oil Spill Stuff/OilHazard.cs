using System.Collections;
using UnityEngine;

public class OilHazard : MonoBehaviour
{
    [Header("Knockback Settings")]
    [Tooltip("Force applied to push the player back")]
    public float knockbackForce = 500f;

    [Tooltip("How long the player is stunned and can't move")]
    public float stunDuration = 3f;

    [Tooltip("How long before player can be hit again by oil")]
    public float cooldownDuration = 0.5f;

    [Header("Ragdoll Settings")]
    [Tooltip("Reduce joint strength to make player go limp (0 = fully limp)")]
    public float limpJointStrength = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a player
        RagdollController ragdollController = collision.gameObject.GetComponentInParent<RagdollController>();

        if (ragdollController != null)
        {
            // Check if player is already stunned (to prevent multiple hits)
            OilStunEffect existingStun = ragdollController.GetComponent<OilStunEffect>();
            if (existingStun != null && existingStun.IsStunned)
            {
                return; // Player is already stunned, don't hit them again
            }

            // Calculate knockback direction (away from oil)
            Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
            knockbackDirection.y = 0.5f; // Add some upward force

            // Apply the knockback and stun effect
            ApplyOilEffect(ragdollController, knockbackDirection);
            Destroy(gameObject); // Destroy oil hazard after hitting a player
        }
    }

    private void ApplyOilEffect(RagdollController ragdollController, Vector3 knockbackDirection)
    {
        // Get or add the stun effect component
        OilStunEffect stunEffect = ragdollController.GetComponent<OilStunEffect>();
        if (stunEffect == null)
        {
            stunEffect = ragdollController.gameObject.AddComponent<OilStunEffect>();
        }

        // Apply knockback force to all limbs
        foreach (var jointPair in ragdollController.RagdollDict)
        {
            RagdollJoint joint = jointPair.Value;
            if (joint != null && joint.Rigidbody != null)
            {
                joint.Rigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }

        // Start the stun effect
        stunEffect.StartStun(stunDuration, cooldownDuration, limpJointStrength);
    }
}
