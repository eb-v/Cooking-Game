 using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class LobbyUIEventHandler : MonoBehaviour
{
    [Header("References")]
    public List<GameObject> uiGameobjects = new List<GameObject>();   // put your “ChangeColor / ChangeFace” rows here (for THIS player column only)
    [SerializeField] private GameObject _firstSelected;                // drag the first row object here
    [SerializeField] private EventSystem _eventSystem;                 // drag the scene EventSystem
    [SerializeField] private CosmeticChanger _cosmeticChanger;         // drag the SAME PLAYER’s CosmeticChanger

    [Header("Assigned Player Values")]
    [SerializeField] private string _assignedPlayerName = "Player 1";  // unique label this UI listens for
    public GameObject assignedPlayerObj;                               // set automatically on join

    [Header("Input")]
    [SerializeField] private PlayerInput _playerInput;                 // set on join
    private InputAction navigateAction, nextOptionAction, previousOptionAction, readyAction;

    private int _uiIndex = 0;
    [SerializeField] private GameObject _currentSelectedObj;
    [SerializeField] private GameObject _previousSelectedObj;

    [Header("Lobby Lock")]
    [SerializeField] private bool _freezeRigidbody = true;
    [SerializeField] private string[] _componentsToDisableOnPlayer = {
        "PlayerMovement", "ThirdPersonController", "CharacterControllerDriver"
    };

    private void Awake()
    {
        if (_eventSystem == null) _eventSystem = EventSystem.current;
        GenericEvent<OnPlayerJoinedEvent>.GetEvent(_assignedPlayerName)
            .AddListener(AssignPlayerInputValues);
    }

    private void OnEnable()
    {
        if (_eventSystem == null) _eventSystem = EventSystem.current;

        if (uiGameobjects.Count > 0)
        {
            _uiIndex = Mathf.Clamp(_uiIndex, 0, uiGameobjects.Count - 1);
            if (_firstSelected == null) _firstSelected = uiGameobjects[0];
            SetSelectedUIObj(_uiIndex);
            StartCoroutine(SelectAfterDelay());
        }
    }

    private void OnDisable()
    {
        if (_playerInput == null) return;
        if (navigateAction != null)        { navigateAction.performed        -= OnNavigate;        navigateAction.Disable(); }
        if (nextOptionAction != null)      { nextOptionAction.performed      -= OnNextOption;      nextOptionAction.Disable(); }
        if (previousOptionAction != null)  { previousOptionAction.performed  -= OnPreviousOption;  previousOptionAction.Disable(); }
        if (readyAction != null)           { readyAction.performed           -= OnReadyInput;      readyAction.Disable(); }
    }

    private IEnumerator SelectAfterDelay()
    {
        yield return null; // wait a frame so ES is ready
        if (_eventSystem && _firstSelected)
        {
            _eventSystem.firstSelectedGameObject = _firstSelected;
            _eventSystem.SetSelectedGameObject(_firstSelected);
        }
    }

    private void SetSelectedUIObj(int index)
    {
        if (uiGameobjects == null || uiGameobjects.Count == 0) return;

        index = Mathf.Clamp(index, 0, uiGameobjects.Count - 1);
        var candidate = uiGameobjects[index];
        if (candidate == null) return;

        _currentSelectedObj = candidate;

        if (_previousSelectedObj && _previousSelectedObj != _currentSelectedObj)
            _previousSelectedObj.transform.localScale = Vector3.one;
        _currentSelectedObj.transform.localScale = Vector3.one * 1.05f;

        _previousSelectedObj = _currentSelectedObj;

        if (_eventSystem) _eventSystem.SetSelectedGameObject(_currentSelectedObj);
    }

    // ===== Input callbacks =====
    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (uiGameobjects == null || uiGameobjects.Count == 0) return;
        var v = ctx.ReadValue<Vector2>().y;
        if (v >  0.2f) { _uiIndex = Mathf.Clamp(_uiIndex - 1, 0, uiGameobjects.Count - 1); SetSelectedUIObj(_uiIndex); }
        if (v < -0.2f) { _uiIndex = Mathf.Clamp(_uiIndex + 1, 0, uiGameobjects.Count - 1); SetSelectedUIObj(_uiIndex); }
    }

    private void OnNextOption(InputAction.CallbackContext ctx)
    {
        if (_currentSelectedObj == null || _cosmeticChanger == null || assignedPlayerObj == null) return;

        var name = _currentSelectedObj.name;
        if (name.Contains("ChangeColor"))
        {
            _cosmeticChanger.NextColor();
            _cosmeticChanger.ApplyColor(assignedPlayerObj);
        }
        else if (name.Contains("ChangeFace"))
        {
            _cosmeticChanger.NextFace();
            _cosmeticChanger.ApplyFace(assignedPlayerObj);
        }
    }

    private void OnPreviousOption(InputAction.CallbackContext ctx)
    {
        if (_currentSelectedObj == null || _cosmeticChanger == null || assignedPlayerObj == null) return;

        var name = _currentSelectedObj.name;
        if (name.Contains("ChangeColor"))
        {
            _cosmeticChanger.PreviousColor();
            _cosmeticChanger.ApplyColor(assignedPlayerObj);
        }
        else if (name.Contains("ChangeFace"))
        {
            _cosmeticChanger.PreviousFace();
            _cosmeticChanger.ApplyFace(assignedPlayerObj);
        }
    }

    private void LockPlayerForLobby()
{
    if (!assignedPlayerObj) return;

    // Freeze Rigidbody
    var rb = assignedPlayerObj.GetComponent<Rigidbody>();
    if (rb && _freezeRigidbody)
    {
        rb.linearVelocity = Vector3.zero;   // (or rb.velocity if using Built-in)
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    // Disable movement scripts by name
    var behaviours = assignedPlayerObj.GetComponents<Behaviour>();
    foreach (var b in behaviours)
    {
        if (!b) continue;
        for (int i = 0; i < _componentsToDisableOnPlayer.Length; i++)
            if (b.GetType().Name == _componentsToDisableOnPlayer[i]) b.enabled = false;
    }
}

    private void OnReadyInput(InputAction.CallbackContext ctx)
    {
        GenericEvent<PlayerReadyEvent>.GetEvent(gameObject.name).Invoke();
    }

    private void AssignPlayerInputValues(PlayerInput playerInput)
{
    _playerInput = playerInput;
    assignedPlayerObj = playerInput.gameObject;

    // IMPORTANT: switch to Lobby action map
    var lobbyMap = _playerInput.actions.FindActionMap("Lobby");
    if (lobbyMap == null) { Debug.LogError("Lobby action map not found"); return; }
    _playerInput.SwitchCurrentActionMap("Lobby");

    // Bind actions (same as you had) ...
    navigateAction       = lobbyMap.FindAction("Navigate");
    nextOptionAction     = lobbyMap.FindAction("NextOption");
    previousOptionAction = lobbyMap.FindAction("PreviousOption");
    readyAction          = lobbyMap.FindAction("ReadyInput");
    if (navigateAction == null || nextOptionAction == null || previousOptionAction == null || readyAction == null) return;

    navigateAction.performed       += OnNavigate;
    nextOptionAction.performed     += OnNextOption;
    previousOptionAction.performed += OnPreviousOption;
    readyAction.performed          += OnReadyInput;

    navigateAction.Enable();
    nextOptionAction.Enable();
    previousOptionAction.Enable();
    readyAction.Enable();

    // ← freeze and disable movement here
    LockPlayerForLobby();

    // ensure a valid selection
    if (uiGameobjects.Count > 0) { _uiIndex = 0; SetSelectedUIObj(_uiIndex); StartCoroutine(SelectAfterDelay()); }
}

}