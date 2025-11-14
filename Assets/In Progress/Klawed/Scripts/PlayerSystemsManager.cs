using UnityEngine;

public class PlayerSystemsManager : MonoBehaviour
{
    private static PlayerSystemsManager instance;

    public static PlayerSystemsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindFirstObjectByType<PlayerSystemsManager>();
            }
            return instance;
        }
    }

    public static void TurnOffPlayerMovement(GameObject player)
    {
        RagdollController rc = player.GetComponent<RagdollController>();
        Rigidbody rootRb = rc.GetPelvis().GetComponent<Rigidbody>();
        rootRb.isKinematic = true;
        rc.enabled = false;
    }

    public static void TurnOnPlayerMovement(GameObject player)
    {
        RagdollController rc = player.GetComponent<RagdollController>();
        Rigidbody rootRb = rc.GetPelvis().GetComponent<Rigidbody>();
        rootRb.isKinematic = false;
        rc.enabled = true;
    }
}
