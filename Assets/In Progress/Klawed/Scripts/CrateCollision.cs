using UnityEngine;

public class CrateCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GenericEvent<CrateLandedEvent>.GetEvent(gameObject.name).Invoke();
        }
    }
}
