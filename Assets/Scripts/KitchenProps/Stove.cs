using NUnit.Framework;
using UnityEditor.Rendering;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Burnable))]
public class Stove : MonoBehaviour
{
    [Header("Stove Section")]
    [SerializeField] private Transform objectSnapPoint;
    public Transform ObjectSnapPoint => objectSnapPoint;

    public CookingRecipe _currentRecipe { get; set; }

    public GameObject _currentObject { get; set; }

    public bool hasObject => _currentObject != null;

    private StateMachine<StoveState> _stateMachine;

    [Header("Visuals")]
    public Canvas _stoveUICanvas;
    public Image _stoveUICookFillImage;
    public Image _stoveUIBurnFillImage;

    [Header("States")]
    [SerializeField] private StoveState _idleStateBase;
    [SerializeField] private StoveState _cookingStateBase;
    [SerializeField] private StoveState _burningStateBase;

    [Header("Cooking Settings")]
    [field: SerializeField] public float cookingDuration { get; private set; } = 5f;
    [field: SerializeField] public float burnDuration { get; private set; } = 8f;

    [Header("Removal Settings")]
    [SerializeField] private float forceMultiplier = 10f;

    public StoveState _idleStateInstance { get; private set; }
    public StoveState _cookingStateInstance { get; private set; }
    public StoveState _burningStateInstance { get; private set; }

    private Burnable burnable;


    private void Awake()
    {
        burnable = GetComponent<Burnable>();

        _stateMachine = new StateMachine<StoveState>();

        _idleStateInstance = Instantiate(_idleStateBase);
        _cookingStateInstance = Instantiate(_cookingStateBase);
        _burningStateInstance = Instantiate(_burningStateBase);
    }

    private void Start()
    {
        _idleStateInstance.Initialize(gameObject, _stateMachine);
        _cookingStateInstance.Initialize(gameObject, _stateMachine);
        _burningStateInstance.Initialize(gameObject, _stateMachine);

        _stateMachine.Initialize(_idleStateInstance);
    }

    private void OnEnable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnAltInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnAltInteract);
    }



    public void OnAltInteract(GameObject player)
    {
        if (burnable.IsOnFire)
            return;
    }

    public void OnInteract(GameObject player)
    {
        if (burnable.IsOnFire)
            return;
        _stateMachine.GetCurrentState<StoveState>().InteractLogic(player, this);
    }
    public void Update()
    {
        if (burnable.IsOnFire)
            return;
        _stateMachine.RunUpdateLogic();
    }

    public void FixedUpdate()
    {
        if (burnable.IsOnFire)
            return;
        _stateMachine.RunFixedUpdateLogic();
    }

    public void ChangeState(StoveState newState)
    {
        _stateMachine.ChangeState(newState);
    }


    public bool IsItemCompatible(GameObject objectToCheck)
    {
        if (objectToCheck.GetComponent<IngredientScript>() == null)
            return false;

        IngredientScript ingredient = objectToCheck.GetComponent<IngredientScript>();

        CookingRecipe cookingRecipe = ingredient.recipes.Find(recipe => recipe is CookingRecipe) as CookingRecipe;

        if (cookingRecipe == null)
            return false;

        return true;
    }

    public void RemoveObject(GameObject player)
    {
    }

}
