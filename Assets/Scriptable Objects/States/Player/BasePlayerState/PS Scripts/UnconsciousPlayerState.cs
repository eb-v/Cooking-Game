using UnityEngine;

[CreateAssetMenu(fileName = "PS_Unconscious", menuName = "Scriptable Objects/States/PlayerState/Unconscious")]
public class UnconsciousPlayerState : BasePlayerState
{
    private RagdollController _rc;
    private float unconsciousTime;
    private PlayerData playerData;
    private Player player;

    public override void Enter()
    {
        base.Enter();
        _rc.ActivateRagdoll();
    }

    public override void Exit()
    {
        base.Exit();
        _rc.DeactivateRagdoll();
        unconsciousTime = 0f;
    }

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
        _rc = gameObject.GetComponent<RagdollController>();
        playerData = LoadPlayerData.GetPlayerData();
        player = gameObject.GetComponent<Player>();
    }

    public override void RunFixedUpdateLogic()
    {
        base.RunFixedUpdateLogic();
    }

    public override void RunUpdateLogic()
    {
        base.RunUpdateLogic();
        unconsciousTime += Time.deltaTime;
        if (unconsciousTime >= playerData.UnconsciousDuration)
        {
            player.ChangeState(player._defaultStateInstance);
        }
    }
}
