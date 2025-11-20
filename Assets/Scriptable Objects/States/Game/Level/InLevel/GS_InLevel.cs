using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GS_InLevel", menuName = "Scriptable Objects/States/Game/Level/InLevel")]
public class GS_InLevel : GameState
{
    [SerializeField] private SceneField inLevelScene;

    [SerializeField] private List<GameObject> _levelSpecificManagers;

    public override void Enter()
    {
        base.Enter();

        GameObject rootManager = new GameObject("LevelManagers");
        foreach (GameObject managerPrefab in _levelSpecificManagers)
        {
            GameObject managerInstance = Instantiate(managerPrefab, rootManager.transform);
        }

    }

    public override void Exit()
    {
        base.Exit();
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

    private void InitializeManagers()
    {
        
    }

}
