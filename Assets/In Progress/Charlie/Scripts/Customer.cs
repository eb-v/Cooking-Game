using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Interactable))]
public class Customer : MonoBehaviour, IOrder
{

    [Header("Order Settings")]
    [SerializeField] private string assignedChannel;
    [SerializeField] private Transform handHoldPoint;


    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Image orderImage;
    [SerializeField] private Image bubbleImage;

    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;
    public Image OrderImage => orderImage;

    [Header("Customer States")]
    [SerializeField] private CustomerState inLine;
    [SerializeField] private CustomerState ordering;
    [SerializeField] private CustomerState walkToTable;
    [SerializeField] private CustomerState eating;
    [SerializeField] private CustomerState leaving;


    #region State Instances
    [HideInInspector] public CustomerState _inLineInstance;
    [HideInInspector] public CustomerState _orderingInstance;
    [HideInInspector] public CustomerState _walkToTableInstance;
    [HideInInspector] public CustomerState _eatAtTableInstance;
    [HideInInspector] public CustomerState _leaveInstance;
    #endregion


    [HideInInspector] public Transform targetLine;
    [HideInInspector] public Transform tablePositions;
    [HideInInspector] public Transform exitPoint;

    public int linePosition { get; set; }

    public Transform assignedTable { get; set; }

    public MenuItem currentOrder { get; private set; }

    private GameObject heldFoodItem;

    private StateMachine<CustomerState> _stateMachine;

    [Header("Debug")]
    [ReadOnly]
    [SerializeField] private CustomerState _currentState;
    [ReadOnly]
    [SerializeField] private MenuItem debug_CurrentOrder;


    // FOR THE ROBBER D:
    [Header("Special NPC Settings")]
    public bool isRobber = false;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (agent == null) agent = GetComponentInChildren<NavMeshAgent>();

        _inLineInstance = Instantiate(inLine);
        _orderingInstance = Instantiate(ordering);
        _walkToTableInstance = Instantiate(walkToTable);
        _eatAtTableInstance = Instantiate(eating);
        _leaveInstance = Instantiate(leaving);

        _stateMachine = new StateMachine<CustomerState>();
    }

    void Start()
    {
        _inLineInstance.Initialize(gameObject, _stateMachine);
        _orderingInstance.Initialize(gameObject, _stateMachine);
        _walkToTableInstance.Initialize(gameObject, _stateMachine);
        _eatAtTableInstance.Initialize(gameObject, _stateMachine);
        _leaveInstance.Initialize(gameObject, _stateMachine);

        _stateMachine.Initialize(_inLineInstance);
        _currentState = _stateMachine.GetCurrentState();
    }

    private void OnEnable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnInteract);
    }

    void Update()
    {
        _stateMachine.RunUpdateLogic();
    }

    void FixedUpdate()
    {
        _stateMachine.RunFixedUpdateLogic();
    }

    private void OnInteract(GameObject player)
    {
        _stateMachine.GetCurrentState().InteractLogic(player);
    }


    public void SpawnFoodInHand()
    {
        if (handHoldPoint == null)
        {
            Debug.LogWarning($"{name}: handHoldPoint not assigned!");
            return;
        }

        if (currentOrder != null && currentOrder.GetOrderItemPrefab() != null)
        {
            heldFoodItem = Instantiate(currentOrder.GetOrderItemPrefab(), handHoldPoint.position, handHoldPoint.rotation, handHoldPoint);

            heldFoodItem.transform.localScale *= 3f;

            Rigidbody rb = heldFoodItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;   // animation/NPC movement fully controls it
                rb.useGravity = false;   // prevent it from falling
            }
        
            Debug.Log($"{name} spawned {currentOrder.GetOrderItemPrefab().name} in hand");
        }
    }

    public void DestroyFoodInHand()
    {
        if (heldFoodItem != null)
        {
            Destroy(heldFoodItem);
            heldFoodItem = null;
        }
    }

    public void MoveTo(Vector3 destination)
    {
        Agent.SetDestination(destination);
    }

    public void ChangeState(CustomerState newState)
    {
        _stateMachine.ChangeState(newState);
        _currentState = newState;
    }

    public void SetOrder(MenuItem orderItem) {
        if (isRobber) return;

        currentOrder = orderItem;
        if (orderItem != null && OrderImage != null)
            OrderImage.sprite = orderItem.GetFoodSprite();
        debug_CurrentOrder = currentOrder;
    }

    public void DisplayImage(bool status) {
        if (isRobber) return;
        bubbleImage.enabled = status;
        orderImage.enabled = status;
    }

    public bool AtFrontOfLine()
    {
        return linePosition == 0;
    }

    public bool IsInLine() {
        return _currentState == _inLineInstance;
    }


    public void LeaveImmediately() {
        Agent.isStopped = false;

        Transform leavePoint = CustomerManager.Instance.GetLeavePosition();
        exitPoint = leavePoint;

        ChangeState(_leaveInstance);

        DisplayImage(false);
    }



}
