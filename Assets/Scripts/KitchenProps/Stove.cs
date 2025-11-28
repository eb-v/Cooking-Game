using System;
using UnityEngine;
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

    [Header("Audio")]
    [Tooltip("Assign a Sizzle AudioSource here (with the Sizzle clip). If left empty, one will be created on Awake.")]
    [SerializeField] private AudioSource sizzleSource;

    private void Awake()
    {
        burnable = GetComponent<Burnable>();

        _stateMachine = new StateMachine<StoveState>();

        _idleStateInstance    = Instantiate(_idleStateBase);
        _cookingStateInstance = Instantiate(_cookingStateBase);
        _burningStateInstance = Instantiate(_burningStateBase);

        // If no AudioSource is wired, create a child one for sizzle
        if (sizzleSource == null)
        {
            GameObject sizzleObj = new GameObject("StoveSizzleAudio");
            sizzleObj.transform.SetParent(transform);
            sizzleObj.transform.localPosition = Vector3.zero;

            sizzleSource = sizzleObj.AddComponent<AudioSource>();
            sizzleSource.playOnAwake = false;
            sizzleSource.loop = false;
            sizzleSource.spatialBlend = 0f; // 2D
        }
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
        GenericEvent<OnInteractableInteracted>
            .GetEvent(gameObject.GetInstanceID().ToString())
            .AddListener(OnInteract);

        GenericEvent<OnInteractableAltInteracted>
            .GetEvent(gameObject.GetInstanceID().ToString())
            .AddListener(OnAltInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractableInteracted>
            .GetEvent(gameObject.GetInstanceID().ToString())
            .RemoveListener(OnInteract);

        GenericEvent<OnInteractableAltInteracted>
            .GetEvent(gameObject.GetInstanceID().ToString())
            .RemoveListener(OnAltInteract);

        StopSizzle();
    }

    public void OnAltInteract(GameObject player)
    {
        if (burnable.IsOnFire)
            return;

        // alt-interact logic lives in states if needed
    }

    public void OnInteract(GameObject player)
    {
        if (burnable.IsOnFire)
            return;

        _stateMachine.GetCurrentState<StoveState>().InteractLogic(player, this);
    }

    private void Update()
    {
        if (burnable.IsOnFire)
        {
            // safety: kill sizzle if the stove catches fire
            StopSizzle();
            return;
        }

        _stateMachine.RunUpdateLogic();
    }

    private void FixedUpdate()
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
        CookingRecipe cookingRecipe =
            ingredient.recipes.Find(recipe => recipe is CookingRecipe) as CookingRecipe;

        return cookingRecipe != null;
    }

    public void RemoveObject(GameObject player)
    {
        // if object leaves the stove, stop the sizzle
        StopSizzle();
        // actual physics/grab handled inside state logic
    }

    // ----------------- AUDIO HELPERS -----------------

    public void StartSizzle()
    {
        if (sizzleSource == null)
            return;

        if (!hasObject || burnable.IsOnFire)
            return;

        if (sizzleSource.isPlaying)
            return;

        if (sizzleSource.clip == null)
        {
            Debug.LogWarning("[Stove] Sizzle AudioSource has no clip assigned. Assign the Sizzle clip in the inspector.");
            return;
        }

        sizzleSource.Play();
        Debug.Log("[Stove] Sizzle started.");
    }

    public void StopSizzle()
    {
        if (sizzleSource != null && sizzleSource.isPlaying)
        {
            sizzleSource.Stop();
            Debug.Log("[Stove] Sizzle stopped.");
        }
    }
}
