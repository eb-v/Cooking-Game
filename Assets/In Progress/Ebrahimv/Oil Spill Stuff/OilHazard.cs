using UnityEngine;

public class OilHazard : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a player
        Player player = collision.gameObject.GetComponentInParent<Player>();

        if (player != null)
        {
            // Change player to unconscious state
            player.ChangeState(player._unconsciousStateInstance);

            // Despawn the oil spill
            Destroy(gameObject);
        }
    }
}
