using NUnit.Framework;
using UnityEditor.Rendering;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Stove : MonoBehaviour, IInteractable, IAltInteractable
{
    [field: SerializeField] public Transform objectSnapPoint { get; private set; }

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

    [Header("Cooking Settings")]
    [field: SerializeField] public float cookingDuration { get; private set; } = 5f;
    [field: SerializeField] public float burnDuration { get; private set; } = 8f;

    public StoveState _idleStateInstance { get; private set; }
    public StoveState _cookingStateInstance { get; private set; }


    private void Awake()
    {
        _stateMachine = new StateMachine<StoveState>();

        _idleStateInstance = Instantiate(_idleStateBase);
        _cookingStateInstance = Instantiate(_cookingStateBase);
    }

    private void Start()
    {
        _idleStateInstance.Initialize(gameObject, _stateMachine);
        _cookingStateInstance.Initialize(gameObject, _stateMachine);

        _stateMachine.Initialize(_idleStateInstance);
    }


    public void OnAltInteract(GameObject player)
    {
        _stateMachine.GetCurrentState<StoveState>().AltInteractLogic(player);
    }

    public void OnInteract(GameObject player)
    {
        _stateMachine.GetCurrentState<StoveState>().InteractLogic(player, this);
    }
    private void Update()
    {
        _stateMachine.RunUpdateLogic();
    }

    private void FixedUpdate()
    {
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

    public GameObject RemoveObject(GameObject player)
    {
        IngredientScript ingredient = _currentObject.GetComponent<IngredientScript>();

        GameObject physicsObj = _currentObject.GetComponent<PhysicsTransform>().physicsTransform.gameObject;

        if (physicsObj == null)
        {
            Debug.LogError("Physics obj not found when removing object from counter");
            return null;
        }


        Rigidbody rb = physicsObj.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        IGrabable grabable = _currentObject.GetComponent<IGrabable>();
        grabable.GrabObject(player);


        _currentObject = null;
        _currentRecipe = null;
        return physicsObj;
    }

}
