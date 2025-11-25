using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AssemblyStation : MonoBehaviour, IInteractable, IAltInteractable
{
    [Header("Settings")]
    [SerializeField] private float timeToAssemble = 7f;
    [field: SerializeField] public float xOffsetRangeMin { get; private set; } = 0.3f;
    [field: SerializeField] public float xOffsetRangeMax { get; private set; } = 0.5f;
    [field: SerializeField] public float zOffsetRangeMin { get; private set; } = 0.3f;
    [field: SerializeField] public float zOffsetRangeMax { get; private set; } = 0.5f;
    [field: SerializeField] public float launchForce { get; private set; } = 2f;
    public float TimeToAssemble => timeToAssemble;

    [Header("References")]
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private GameObject progressBar;
    [SerializeField] private Image progressFillImage;
    public Vector3 SpawnPosition => spawnPosition.position;
    public GameObject ProgressBar => progressBar;
    public Image FillImg => progressFillImage;

    [Header("States")]
    [SerializeField] private AssemblyState idleState;
    [SerializeField] private AssemblyState assembleState;

    [Header("Storage")]
    [ReadOnly]
    public UDictionary<Ingredient, int> ingredientStorage = new UDictionary<Ingredient, int>();
    public List<Ingredient> availableIngredients = new List<Ingredient>();
    public List<MenuItem> availableMenuItems = new List<MenuItem>();

    [Header("Debug")]
    [ReadOnly]
    public MenuItem selectedMenuItem;


    public int selectedMenuItemIndex { get; set; }
    public MenuItem SelectedMenuItem => selectedMenuItem;

    public StateMachine<AssemblyState> stateMachine { get; private set; }

    #region State Instances
    public AssemblyState IdleStateInstance { get; private set; }
    public AssemblyState AssembleStateInstance { get; private set; }
    #endregion


    private void Awake()
    {
        Initialize();

        stateMachine = new StateMachine<AssemblyState>();
        IdleStateInstance = Instantiate(idleState);
        AssembleStateInstance = Instantiate(assembleState);
    }

    private void Initialize()
    {

        foreach (Ingredient ingredient in availableIngredients)
        {
            ingredientStorage[ingredient] = 0;
        }

        selectedMenuItem = availableMenuItems[0];
    }

    private void Start()
    {
        IdleStateInstance.Initialize(gameObject, stateMachine);
        AssembleStateInstance.Initialize(gameObject, stateMachine);
        stateMachine.Initialize(IdleStateInstance);
    }

    private void Update()
    {
        stateMachine.RunUpdateLogic();
    }

    private void FixedUpdate()
    {
        stateMachine.RunFixedUpdateLogic();
    }

    public void ChangeSelectedMenuItem()
    {
        selectedMenuItemIndex++;
        if (selectedMenuItemIndex >= availableMenuItems.Count)
        {
            selectedMenuItemIndex = 0;
        }
        selectedMenuItem = availableMenuItems[selectedMenuItemIndex];
    }
    
    public void OnInteract(GameObject player)
    {
        stateMachine.GetCurrentState().InteractLogic(player);
    }

    public void OnAltInteract(GameObject player)
    {
        stateMachine.GetCurrentState().AltInteractLogic(player);
    }

    public void ChangeState(AssemblyState newState)
    {
        stateMachine.ChangeState(newState);
    }

}
