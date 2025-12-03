using UnityEngine;

public class MovementMarker : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerLayerCollider(other))
        {
            GenericEvent<TutorialGoalCompleted>.GetEvent("MovementMarkerReached").Invoke();
        }
    }

    private bool IsPlayerLayerCollider(Collider other)
    {
        return ((1 << other.gameObject.layer) & playerLayer) != 0;
    }
}
