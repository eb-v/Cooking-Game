using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GS_InLevel", menuName = "Scriptable Objects/States/Game/Level/InLevel")]
public class GS_InLevel : GameState
{
    [SerializeField] private SceneField inLevelScene;

    private GameObject _rootManagerObject;
    private List<GameObject> _levelSpecificManagers = new List<GameObject>();

    public override void Enter()
    {
        base.Enter();


        _rootManagerObject = GameObject.Find("Managers");
        if (_rootManagerObject == null)
        {
            Debug.LogError("Ensure there is a GameObject named 'Managers' in the scene.");
        }

        foreach (Transform child in _rootManagerObject.transform)
        {
            _levelSpecificManagers.Add(child.gameObject);
        }
        Debug.Log("Entered In-Level State");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exited In-Level State");
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
