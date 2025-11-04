using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class LobbyUIEventHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected GameObject _firstSelected;
    [SerializeField] protected EventSystem _eventSystem;
    [SerializeField] protected CosmeticChanger _cosmeticChanger;
    [SerializeField] protected List<GameObject> uiGameobjects = new List<GameObject>();

    [Header("Animations")]
    [SerializeField] protected float _selectedAnimationScale = 1.2f;
    [SerializeField] protected float _deselectedAnimationScale = 0.8f;
    [SerializeField] protected float _multiplier = 1f;

    [Header("Assigned Player Values")]
    [SerializeField] protected string _assignedPlayerName = "Player 1";
    public GameObject assignedPlayerObj;

    [Header("Player Input Component")]
    [SerializeField] private PlayerInput _playerInput;

    private InputAction navigateAction;
    private InputAction nextOptionAction;
    private InputAction previousOptionAction;
    private InputAction readyAction;

    [SerializeField] private int _uiIndex = 0;
    [SerializeField] private GameObject _currentSelectedObj;
    [SerializeField] private GameObject _previousSelectedObj;


    public virtual void Awake()
    {
        GenericEvent<OnPlayerJoinedEvent>.GetEvent(_assignedPlayerName).AddListener(AssignPlayerInputValues);
    }

    public virtual void Initialize(PlayerInput playerInput)
    {
        AssignPlayerInputValues(playerInput);
    }



    public virtual void OnEnable()
    {
        _uiIndex = 0;
        SetSelectedUIObj(_uiIndex);
        navigateAction.performed += OnNavigate;
        nextOptionAction.performed += OnNextOption;
        previousOptionAction.performed += OnPreviousOption;
        readyAction.performed += OnReady;

        navigateAction.Enable();
        nextOptionAction.Enable();
        previousOptionAction.Enable();
        readyAction.Enable();
    }

    public virtual void OnDisable()
    {
        if (_playerInput == null) return;

        navigateAction.performed -= OnNavigate;
        nextOptionAction.performed -= OnNextOption;
        previousOptionAction.performed -= OnPreviousOption;
        readyAction.performed -= OnReady;

        navigateAction.Disable();
        nextOptionAction.Disable();
        previousOptionAction.Disable();
        readyAction.Disable();
    }

    protected virtual IEnumerator SelectAfterDelay()
    {
        yield return null;
        _eventSystem.SetSelectedGameObject(_firstSelected.gameObject);
    }


    protected virtual void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().y > 0)
        {
            _uiIndex--;
            _uiIndex = Mathf.Clamp(_uiIndex, 0, uiGameobjects.Count - 1);
            SetSelectedUIObj(_uiIndex);
        }
        else if (context.ReadValue<Vector2>().y < 0)
        {
            _uiIndex++;
            _uiIndex = Mathf.Clamp(_uiIndex, 0, uiGameobjects.Count - 1);
            SetSelectedUIObj(_uiIndex);
        }
    }



    protected virtual void OnNextOption(InputAction.CallbackContext context)
    {
        // handle visuals for next option button press
        NextPreviousButton nextPrevButtons = _currentSelectedObj.GetComponent<NextPreviousButton>();
        SpringAPI springAPI = nextPrevButtons.nextButton.GetComponent<SpringAPI>();
        springAPI.NudgeSpringVelocity();

        // find out what specific option is selected
        if (_currentSelectedObj.name.Contains("ChangeColor"))
        {
            //if (_cosmeticChanger == null)
            //{
            //    Debug.LogWarning("CosmeticChanger reference is missing!");
            //    return;
            //}
            _cosmeticChanger.NextColor();
            _cosmeticChanger.ChangeRobotColor(assignedPlayerObj);
        }
        else if (_currentSelectedObj.name.Contains("ChangeFace"))
        {
            _cosmeticChanger.NextFace();
            _cosmeticChanger.ChangeRobotFace(assignedPlayerObj);
        }

    }

    protected virtual void OnPreviousOption(InputAction.CallbackContext context)
    {
        // handle visuals for previous option button press
        NextPreviousButton nextPrevButtons = _currentSelectedObj.GetComponent<NextPreviousButton>();
        SpringAPI springAPI = nextPrevButtons.previousButton.GetComponent<SpringAPI>();
        springAPI.NudgeSpringVelocity();

        // find out what specific option is selected
        if (_currentSelectedObj.name.Contains("ChangeColor"))
        {
            _cosmeticChanger.PreviousColor();
            _cosmeticChanger.ChangeRobotColor(assignedPlayerObj);
        }
        else if (_currentSelectedObj.name.Contains("ChangeFace"))
        {
            _cosmeticChanger.PreviousFace();
            _cosmeticChanger.ChangeRobotFace(assignedPlayerObj);
        }
    }

    protected virtual void OnReady(InputAction.CallbackContext context)
    {
        if (readyAction == null)
        {
            Debug.LogWarning("Ready action is not assigned.");
            return;
        }
        
        GenericEvent<PlayerReadyInputEvent>.GetEvent(gameObject.name).Invoke();
    }

    private void AssignPlayerInputValues(PlayerInput playerInput)
    {
        _playerInput = playerInput;
        assignedPlayerObj = playerInput.gameObject;

        var lobbyActionMap = _playerInput.actions.FindActionMap("Lobby");

        navigateAction = lobbyActionMap.FindAction("Navigate");
        nextOptionAction = lobbyActionMap.FindAction("NextOption");
        previousOptionAction = lobbyActionMap.FindAction("PreviousOption");
        readyAction = lobbyActionMap.FindAction("ReadyInput");

        if (readyAction == null)
        {
            Debug.LogWarning("Ready action not found in Lobby action map.");
        }

        //gameObject.SetActive(true);
    }


    public void SetSelectedUIObj(int index)
    {
        _currentSelectedObj = uiGameobjects[index];

        if (_currentSelectedObj == _previousSelectedObj) return;


        SpringAPI springAPI = _currentSelectedObj.GetComponent<SpringAPI>();

        // enlarge selected object
        springAPI.SetGoalValue(1f);

        // shrink previous selected object
        if (_previousSelectedObj != null)
        {
            SpringAPI previousSelectSpringAPI = _previousSelectedObj.GetComponent<SpringAPI>();
            previousSelectSpringAPI.SetGoalValue(0f);
        }
        _previousSelectedObj = _currentSelectedObj;
    }


}