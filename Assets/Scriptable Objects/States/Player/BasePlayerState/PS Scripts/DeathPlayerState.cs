using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PS_Dead", menuName = "Scriptable Objects/States/PlayerState/Dead")]
public class DeathPlayerState : BasePlayerState
{
    [SerializeField] private float despawnDelay = 8f;

    public override void Enter()
    {
        base.Enter();
        CoroutineRunner.Instance.StartCoroutine(DespawnCoroutine());
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
    }
    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSeconds(despawnDelay);
        PlayerManager.Instance.RespawnPlayer(gameObject);
    }



}
