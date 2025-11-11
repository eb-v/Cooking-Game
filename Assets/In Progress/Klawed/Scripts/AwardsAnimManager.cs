using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class AwardsAnimManager : MonoBehaviour
{
    [SerializeField] private AwardsAnimData awardsAnimData;
    [SerializeField] private List<GameObject> playersInScene;
    private void Start()
    {
        //foreach (var player in playersInScene)
        //{
        //    int index = playersInScene.IndexOf(player);
        //    //player.GetComponent<RagdollController>().enabled = false;
        //    //foreach (Collider col in player.GetComponentsInChildren<Collider>())
        //    //{
        //    //    col.enabled = false;
        //    //}
        //    player.SetActive(false);
        //    player.GetComponent<RagdollController>().GetPelvis().GetComponent<Rigidbody>().isKinematic = true;
        //    player.SetActive(true);

        //}

    }

}
