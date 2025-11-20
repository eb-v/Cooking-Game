using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GS_Lobby", menuName = "Scriptable Objects/States/Game/Lobby")]
public class GS_Lobby : GameState
{
    [SerializeField] private SceneField lobbyScene;

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entered Lobby State");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exited Lobby State");
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
