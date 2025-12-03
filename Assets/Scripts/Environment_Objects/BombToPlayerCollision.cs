using UnityEngine;

public class BombToPlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject rootObj = other.transform.root.gameObject;
        Debug.Log("Bomb collided with player, invoking event.");
        if (rootObj.TryGetComponent<Player>(out Player player))
        {
            GenericEvent<BombCollidedWithPlayer>.GetEvent(transform.parent.GetInstanceID().ToString()).Invoke();
            Debug.Log("Bomb collided with player, invoking event.");
        }
    }
}
