using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("All Game States")]
    [SerializeField] private GameState _mainMenuState;
    [SerializeField] private GameState _lobbyState;
    [SerializeField] private GameState _preLevelState;
    [SerializeField] private GameState _inLevelState;
    [SerializeField] private GameState _postLevelState;

    [Header("Settings")]
    [SerializeField] private string _startingState;


    private static StateMachine<GameState> _stateMachine;

    #region State Instances
    [HideInInspector] public GameState _mainMenuStateInstance;
    [HideInInspector] public GameState _lobbyStateInstance;
    [HideInInspector] public GameState _preLevelStateInstance;
    [HideInInspector] public GameState _inLevelStateInstance;
    [HideInInspector] public GameState _postLevelStateInstance;
    #endregion

    [Header("Debugging")]
    [ReadOnly]
    [SerializeField] private GameState _currentState;

    public static GameManager Instance { get; private set; }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _stateMachine = new StateMachine<GameState>();
        _mainMenuStateInstance = Instantiate(_mainMenuState);
        _lobbyStateInstance = Instantiate(_lobbyState);
        _preLevelStateInstance = Instantiate(_preLevelState);
        _inLevelStateInstance = Instantiate(_inLevelState);
        _postLevelStateInstance = Instantiate(_postLevelState);

        _mainMenuStateInstance.Initialize(this.gameObject, _stateMachine);
        _lobbyStateInstance.Initialize(this.gameObject, _stateMachine);
        _preLevelStateInstance.Initialize(this.gameObject, _stateMachine);
        _inLevelStateInstance.Initialize(this.gameObject, _stateMachine);
        _postLevelStateInstance.Initialize(this.gameObject, _stateMachine);
    }


    private void Start()
    {
        switch (_startingState)
        {
            case "MainMenu":
                _stateMachine.Initialize(_mainMenuStateInstance);
                break;
            case "Lobby":
                _stateMachine.Initialize(_lobbyStateInstance);
                break;
            case "PreLevel":
                _stateMachine.Initialize(_preLevelStateInstance);
                break;
            case "InLevel":
                _stateMachine.Initialize(_inLevelStateInstance);
                break;
            case "PostLevel":
                _stateMachine.Initialize(_postLevelStateInstance);
                break;
            default:
                Debug.LogError("Invalid starting state");
                break;
        }
        _currentState = _stateMachine.GetCurrentState();
    }


    private void Update()
    {
        _stateMachine.RunUpdateLogic();
    }

    private void FixedUpdate()
    {
        _stateMachine.RunFixedUpdateLogic();
    }

    public GameState GetCurrentState()
    {
        return _stateMachine.GetCurrentState();
    }

    public void ChangeState(GameState newState)
    {
        _stateMachine.ChangeState(newState);
        _currentState = _stateMachine.GetCurrentState();
    }
    // Switch scene without changing state
    public void SwitchScene(SceneField sceneToUnload, SceneField sceneToLoad)
    {
        StartCoroutine(SwitchScenesCoroutine(sceneToUnload, sceneToLoad));
    }

    // Switch scene and change state after loading
    public void SwitchScene(SceneField sceneToUnload, SceneField sceneToLoad, GameState newState)
    {
        StartCoroutine(SwitchScenesAndChangeStateCoroutine(sceneToUnload, sceneToLoad, newState));
    }


    public void MoveObjectToScene(GameObject objectToMove, SceneField scene)
    {
        SceneManager.MoveGameObjectToScene(objectToMove, SceneManager.GetSceneByName(scene));
    }


    private IEnumerator SwitchScenesAndChangeStateCoroutine(SceneField sceneToUnload, SceneField sceneToLoad, GameState newState)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneToUnload);
        while (!loadOp.isDone || !unloadOp.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
        ChangeState(newState);
    }

    private IEnumerator SwitchScenesCoroutine(SceneField sceneToUnload, SceneField sceneToLoad)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneToUnload);
        while (!loadOp.isDone || !unloadOp.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
    }

    public void LoadSceneAdditively(SceneField sceneToLoad)
    {
        StartCoroutine(LoadSceneAdditivelyCoroutine(sceneToLoad));
    }

    private IEnumerator LoadSceneAdditivelyCoroutine(SceneField sceneToLoad)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        while (!loadOp.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
    }

    public void UnloadAdditiveScene(SceneField sceneToUnload)
    {
        StartCoroutine(UnloadAdditiveSceneCoroutine(sceneToUnload));
    }

    private IEnumerator UnloadAdditiveSceneCoroutine(SceneField sceneToUnload)
    {
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneToUnload);
        while (!unloadOp.isDone)
        {
            yield return null;
        }

    }

    public SceneField GetCurrentActiveScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneField sceneField = new SceneField(currentScene);
        return sceneField;
    }


    public void RunOperations(List<AsyncOperation> operations)
    {
        StartCoroutine(RunAsyncOperations(operations));
    }

    private IEnumerator RunAsyncOperations(List<AsyncOperation> operations)
    {
        for (int i = 0; i < operations.Count; i++)
        {
            while (!operations[i].isDone)
            {
                yield return null;
            }
        }
    }

    

}
