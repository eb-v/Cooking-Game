using UnityEngine;

public class PlayZone : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            GameObject playerRootObj = other.gameObject.transform.root.gameObject;
            if (playerRootObj.TryGetComponent<Player>(out Player player))
            {
                player.Die();
            }

        }

    }

    private bool IsPlayer(GameObject obj)
    {
        return (playerLayer.value & (1 << obj.layer)) > 0;
    }
}
