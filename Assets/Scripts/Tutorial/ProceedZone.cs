using System.Collections.Generic;
using UnityEngine;

public class ProceedZone : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    private HashSet<Player> playersInZone = new HashSet<Player>();
    [SerializeField] private int playersInsideZone = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(IsPlayerLayerCollider(other))
        {
            GameObject playerRootObject = other.transform.root.gameObject;
            if (playerRootObject.TryGetComponent<Player>(out Player player))
            {
                if (!playersInZone.Contains(player))
                {
                    playersInZone.Add(player);

                    if (AllPlayersInZone())
                    {
                        GenericEvent<AllPlayersStandingInProceedZone>.GetEvent("TutorialManager").Invoke();
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayerLayerCollider(other))
        {
            GameObject playerRootObject = other.transform.root.gameObject;
            if (playerRootObject.TryGetComponent<Player>(out Player player))
            {
                if (playersInZone.Contains(player))
                {
                    playersInZone.Remove(player);
                }
            }
        }
    }

    private void Update()
    {
        playersInsideZone = playersInZone.Count;
    }

    private bool IsPlayerLayerCollider(Collider other)
    {
        return ((1 << other.gameObject.layer) & playerLayer) != 0;
    }

    private bool AllPlayersInZone()
    {
        if (playersInZone.Count < PlayerManager.Instance.PlayerCount)
        {
            return false;
        }

        return true;
    }
}
