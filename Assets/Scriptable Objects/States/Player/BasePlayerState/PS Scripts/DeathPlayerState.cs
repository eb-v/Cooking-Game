using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PS_Dead", menuName = "Scriptable Objects/States/PlayerState/Dead")]
public class DeathPlayerState : BasePlayerState
{
    [SerializeField] private float despawnDelay = 8f;
    private float timer = 0f;
    private DeathScript deathScript;
    List<LimbHP> limbHps = new List<LimbHP>();

    public override void Enter()
    {
        base.Enter();
        DisconnectAllLimbs();
        ApplyDeathForces();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
        deathScript = gameObject.GetComponent<DeathScript>();
        foreach (LimbHP limbHp in gameObject.GetComponentsInChildren<LimbHP>())
        {
            limbHps.Add(limbHp);
        }
    }
    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSeconds(despawnDelay);
        PlayerManager.Instance.RespawnPlayer(gameObject);
    }

    public override void RunUpdateLogic()
    {
        base.RunUpdateLogic();

        timer += Time.deltaTime;
        if (timer >= despawnDelay)
        {
            // Respawn player
            PlayerManager.Instance.RespawnPlayer(gameObject);
        }
    }
    private void DisconnectAllLimbs()
    {
        foreach (LimbHP limbHp in limbHps)
        {
            limbHp.DisconnectLimb();
        }
    }

    // calculate random force and direction to apply to limbs upon death
    private void ApplyDeathForces()
    {
        foreach (LimbHP limbHp in limbHps)
        {
            Rigidbody rb = limbHp.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.5f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;
                float randomForceMagnitude = Random.Range(200f, 500f);
                rb.AddForce(randomDirection * randomForceMagnitude);
            }
        }
    }



}
