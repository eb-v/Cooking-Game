using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GS_InLevel", menuName = "Scriptable Objects/States/Game/Level/InLevel")]
public class GS_InLevel : GameState
{
    public override void Enter()
    {
        base.Enter();
        PlayerManager.Instance.MovePlayersToSpawnPositions();
        PlayerManager.Instance.SwitchPlayerActionMaps("Player");
        foreach (GameObject player in PlayerManager.Instance.Players)
        {
            PlayerSystemsManager.TurnOnPlayerMovement(player);
            RagdollController ragdollController= player.GetComponent<RagdollController>();
            ragdollController.SetMovementStatus(true);
        }

    }

    public override void Exit()
    {
        base.Exit();
        foreach (GameObject player in PlayerManager.Instance.Players)
        {
            RagdollController ragdollController = player.GetComponent<RagdollController>();
            ragdollController.SetMovementStatus(false);
        }
    }

    public override void FixedUpdateLogic()
    {
        base.FixedUpdateLogic();
    }

    public override void Initialize(GameObject gameObject, StateMachine<GameState> stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
    }

    public override void PerformSceneTransition()
    {
        base.PerformSceneTransition();
    }

    public override void PlaceHolder()
    {
        base.PlaceHolder();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    

}
