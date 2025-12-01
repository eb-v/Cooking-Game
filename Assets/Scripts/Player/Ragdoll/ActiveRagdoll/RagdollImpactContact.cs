using UnityEngine;

public class RagdollImpactContact : MonoBehaviour
{
    public RagdollController ragdollController;
    public PlayerData playerData;

    private void Start()
    {
        ragdollController = transform.root.GetComponent<RagdollController>();
        playerData = Resources.Load<PlayerData>("Data/Player/PlayerData");
    }

    void OnCollisionEnter(Collision col)
    {
        
        if (ragdollController.canBeKnockoutByImpact && col.relativeVelocity.magnitude > playerData.RequiredForceToBeKO)
        {
            Debug.Log("Ragdoll Impact Contact - Collision detected with force: " + col.relativeVelocity.magnitude);
            ragdollController.ActivateRagdoll();
        }
    }
}
