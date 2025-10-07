using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MechanicalFailureStatus : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private RagdollController ragdollController;
    [SerializeField] private GameObject[] damageableLimbs;
    private Env_Interaction env_interaction;


    private GameObject root;
    private float value;
    public float upwardsExplosiveForce = 500f;
    public float horizontalExplosionForce = 900f;

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

        //ragdoll.ActivateRagdoll();


        // get position of the object the player is looking at
        //Vector3 explosionPos = env_interaction.currentlyLookingAt.transform.position;
        Vector3 explosionPos = ragdollController.GetPlayerPosition();

        // apply explosion force in front of player
        //rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upwardsModifier, ForceMode.Impulse);

        // position in front of the player
        Vector3 backwardsVector = ragdollController.centerOfMass.forward * -1f;


        // apply forces to all the main limbs of the player
        Rigidbody head = ragdollController.RagdollDict["Head"].GetComponent<Rigidbody>();
        Rigidbody leftArm = ragdollController.RagdollDict["UpperLeftArm"].GetComponent<Rigidbody>();
        Rigidbody rightArm = ragdollController.RagdollDict["UpperRightArm"].GetComponent<Rigidbody>();
        Rigidbody leftLeg = ragdollController.RagdollDict["UpperLeftLeg"].GetComponent<Rigidbody>();
        Rigidbody rightLeg = ragdollController.RagdollDict["UpperRightLeg"].GetComponent<Rigidbody>();

        head.AddForce(backwardsVector * horizontalExplosionForce * 0.5f, ForceMode.Impulse);
        head.AddForce(Vector3.up * upwardsExplosiveForce * 0.5f, ForceMode.Impulse);
        leftArm.AddForce(backwardsVector * horizontalExplosionForce * 0.7f, ForceMode.Impulse);
        leftArm.AddForce(Vector3.up * upwardsExplosiveForce * 0.7f, ForceMode.Impulse);
        rightArm.AddForce(backwardsVector * horizontalExplosionForce * 0.7f, ForceMode.Impulse);
        rightArm.AddForce(Vector3.up * upwardsExplosiveForce * 0.7f, ForceMode.Impulse);
        leftLeg.AddForce(backwardsVector * horizontalExplosionForce * 0.9f, ForceMode.Impulse);
        leftLeg.AddForce(Vector3.up * upwardsExplosiveForce * 0.9f, ForceMode.Impulse);
        rightLeg.AddForce(backwardsVector * horizontalExplosionForce * 0.9f, ForceMode.Impulse);
        rightLeg.AddForce(Vector3.up * upwardsExplosiveForce * 0.9f, ForceMode.Impulse);


        // horizontal force
        rb.AddForce(backwardsVector * horizontalExplosionForce, ForceMode.Impulse);
        // vertical force
        rb.AddForce(Vector3.up * upwardsExplosiveForce, ForceMode.Impulse);

        // spawn explosion effect on Player
        Vector3 playerPos = ragdollController.GetPlayerPosition();


        GameObject explosionEffect = ObjectPoolManager.SpawnObject(explosionEffectPrefab, playerPos, Quaternion.identity);

        ResetInstability();

        // invoke event to apply damage to limbs
        ApplyDamageToLimbs();
    }

    private void ResetInstability()
    {
        mechanicalFailureInstability = 0f;
    }

    private void ApplyDamageToLimbs()
    {
        // Invoke event to apply damage to each limb
        foreach (GameObject limb in damageableLimbs)
        {
            float damageAmount = Random.Range(20f, 50f);
            GenericEvent<LimbDamaged>.GetEvent(gameObject.name + limb.name).Invoke(damageAmount);
        }
    }

   

}
