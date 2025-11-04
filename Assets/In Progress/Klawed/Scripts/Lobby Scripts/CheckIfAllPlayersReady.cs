using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CheckIfAllPlayersReady : MonoBehaviour
{
    [SerializeField] private int totalPlayers = 4;
    [SerializeField] private int readyPlayers = 0;
    [SerializeField] private string assignedChannel = "DefaultChannel";

    [SerializeField] private List<PlayerReadyScript> playerReadyScripts = new List<PlayerReadyScript>();

    private void Start()
    {
        //StartCoroutine(forceReadycoroutine());
    }

    public void IncrementReady()
    {
        readyPlayers++;
        CheckAllReady();
    }

    public void DecrementReady()
    {
        readyPlayers--;
    }

    private void CheckAllReady()
    {
        if (readyPlayers >= totalPlayers)
        {
            GenericEvent<OnAllPlayersReadyEvent>.GetEvent(assignedChannel).Invoke();
        }
    }

    public void ForceAllReady()
    {
        foreach (var prs in playerReadyScripts)
        {
            prs.OnPlayerReady();
        }
    }

    private IEnumerator forceReadycoroutine()
    {
        yield return new WaitForSeconds(3f);
        ForceAllReady();
    }
}
