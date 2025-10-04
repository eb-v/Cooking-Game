using System.Runtime.CompilerServices;
using UnityEngine;

public class MechanicalFailureStatus : MonoBehaviour
{
    [SerializeField] private float explosionForce;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private RagdollController ragdollController;
    private Env_Interaction env_interaction;


    public float explosionRadius = 5f;
    private GameObject root;
    private float value;
    public float upwardsModifier = 0.5f;

    // this variable builds up over time when player fails skill checks
    [SerializeField, Tooltip("RunTime value - do not edit!")]
    private float mechanicalFailureInstability = 0f;

    private void Awake()
    {
        GenericEvent<SkillCheckAttemptFailed>.GetEvent(gameObject.name).AddListener(IncreaseInstability);
        GenericEvent<OnExplodeInput>.GetEvent(gameObject.name).AddListener(Explode);

        env_interaction = GetComponent<Env_Interaction>();
        root = gameObject.transform.Find("DEF_Pelvis").gameObject;
    }

    private void IncreaseInstability(float amount)
    {
        mechanicalFailureInstability += amount;

        if (CheckForMechanicalFailure())
        {
            Explode();
        }

    }

    private bool CheckForMechanicalFailure()
    {
        value = Random.Range(0f, 100f);
        return value < mechanicalFailureInstability;
    }

    // you don't want this to happen
    private void Explode()
    {
        Rigidbody rb = root.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("MechanicalFailureStatus: No Rigidbody found on root object!");
            return;
        }
        RagdollController ragdoll = GetComponent<RagdollController>();

        //ragdoll.ActivateRagdoll();


        // get position of the object the player is looking at
        //Vector3 explosionPos = env_interaction.currentlyLookingAt.transform.position;
        Vector3 explosionPos = ragdollController.GetPlayerPosition();

        // apply explosion force in front of player
        rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upwardsModifier, ForceMode.Impulse);


        // spawn explosion effect on Player
        Vector3 playerPos = ragdollController.GetPlayerPosition();


        GameObject explosionEffect = ObjectPoolManager.SpawnObject(explosionEffectPrefab, playerPos, Quaternion.identity);

        ResetInstability();

    }

    private void ResetInstability()
    {
        mechanicalFailureInstability = 0f;
    }

}
