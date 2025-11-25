using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GS_MainMenu", menuName = "Scriptable Objects/States/Game/MainMenu")]
public class GS_MainMenu : GameState
{
    [SerializeField] private SceneField mainMenuScene;

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entered Main Menu State");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exited Main Menu State");
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
