using UnityEngine;

public class ProceedZone : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [ReadOnly]
    [SerializeField] private int numOfPlayersInZone = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(IsPlayerLayerCollider(other))
        {
            numOfPlayersInZone++;
            if (AllPlayersInZone())
            {
                GenericEvent<AllPlayersStandingInProceedZone>.GetEvent("TutorialManager").Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayerLayerCollider(other))
        {
            numOfPlayersInZone--;
        }
    }

    private bool IsPlayerLayerCollider(Collider other)
    {
        return ((1 << other.gameObject.layer) & playerLayer) != 0;
    }

    private bool AllPlayersInZone()
    {
        if (numOfPlayersInZone < PlayerManager.Instance.PlayerCount)
        {
            return false;
        }

        return true;
    }
}
