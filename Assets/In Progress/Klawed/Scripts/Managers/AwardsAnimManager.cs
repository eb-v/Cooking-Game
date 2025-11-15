using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class AwardsAnimManager : MonoBehaviour
{
    [SerializeField] private AwardsAnimData awardsAnimData;
    [SerializeField] private List<GameObject> playersInScene;
    private void Start()
    {
        GameObject playerContainer = GameObject.Find("Players");

        foreach (Transform player in playerContainer.transform)
        {
            playersInScene.Add(player.gameObject);
        }

    }

}
