using UnityEngine;

public class MovementMarker : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    private bool hasBeenReached = false;

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerLayerCollider(other) && !hasBeenReached)
        {
            hasBeenReached = true;
            GenericEvent<TutorialGoalCompleted>.GetEvent("MovementMarkerReached").Invoke();
            gameObject.SetActive(false);
        }
    }

    private bool IsPlayerLayerCollider(Collider other)
    {
        return ((1 << other.gameObject.layer) & playerLayer) != 0;
    }
}
