using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DeathScript : MonoBehaviour
{
    private RagdollController rc;
    private Player player;
    private List<LimbHP> limbHps = new List<LimbHP>();

    private void Awake()
    {
        rc = GetComponent<RagdollController>();
        foreach (LimbHP limbHp in GetComponentsInChildren<LimbHP>())
        {
            limbHps.Add(limbHp);
        }

        player = GetComponent<Player>();
    }


    public void Die()
    {
        foreach (LimbHP limbHp in limbHps)
        {
            limbHp.DisconnectLimb();
        }
        player.ChangeState(player._deathStateInstance);
    }
}
