using UnityEngine;

public class Respawn : MonoBehaviour
{
    private void Start()
    {
        GenericEvent<OnRespawnInput>.GetEvent("RespawnManager").AddListener(OnRespawn);
    }



    private void OnRespawn(GameObject player)
    {
        Debug.Log("Respawning player: " + player.name);

        // Play respawn sound via AudioManager
        AudioManager.Instance?.PlaySFX("Respawn");

        // Optionally reset velocity if there's a Rigidbody component
        RagdollController rc = player.GetComponent<RagdollController>();

        Rigidbody rootRb = rc.GetPelvis().GetComponent<Rigidbody>();
        Transform playerTransform = rootRb.transform;
        playerTransform.position = transform.position;

        rc.ActivateRagdoll();

        rootRb.linearVelocity = Vector3.zero;
        rootRb.angularVelocity = Vector3.zero;
    }
}
