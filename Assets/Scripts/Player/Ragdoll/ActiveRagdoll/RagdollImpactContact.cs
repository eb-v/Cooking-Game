using UnityEngine;

public class RagdollImpactContact : MonoBehaviour
{
    public RagdollController ragdollController;

    private void Start()
    {
        ragdollController = transform.root.GetComponent<RagdollController>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (ragdollController.canBeKnockoutByImpact && col.relativeVelocity.magnitude > ragdollController.requiredForceToBeKO)
        {
            Debug.Log("Ragdoll Impact Contact - Collision detected with force: " + col.relativeVelocity.magnitude);
            ragdollController.ActivateRagdoll();
        }
    }
}
