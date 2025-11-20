using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour, IInteractable
{
    public NPCState currentState;
    public NPC npc;
    public NpcManager manager;

    [Header("Order Settings")]
    [SerializeField] private string assignedChannel;
    [SerializeField] private Transform handHoldPoint;

    private NpcOrderScript npcOrderScript;

    [Header("References")]
    [field: SerializeField] public NavMeshAgent agent { get; private set; }
    [field: SerializeField] public Animator animator { get; private set; }
    [field: SerializeField] public GameObject npcCanvas { get; private set; }


    [Header("Customer States")]
    [SerializeField] private CustomerState ordering;
    [SerializeField] private CustomerState walkToTable;
    [SerializeField] private CustomerState eating;
    [SerializeField] private CustomerState leaving;


    #region State Instances
    [HideInInspector] public CustomerState _orderingInstance;
    [HideInInspector] public CustomerState _walkToTableInstance;
    [HideInInspector] public CustomerState _eatAtTableInstance;
    [HideInInspector] public CustomerState _leaveInstance;
    #endregion


    [HideInInspector] public Transform targetLine;
    [HideInInspector] public Transform tablePositions;
    [HideInInspector] public Transform exitPoint;

    public Transform assignedTable { get; set; }
    private Transform leavePosition;
    private bool hasReceivedOrder = false;
    private bool isEating = false;
    private GameObject heldFoodItem;

    private StateMachine<CustomerState> _stateMachine;

    [Header("Debug")]
    [ReadOnly]
    [SerializeField] private CustomerState _currentState;
    [ReadOnly]
    [SerializeField] private MenuItem customersOrder;
    public MenuItem CustomersOrder => customersOrder;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();


        _orderingInstance = Instantiate(ordering);
        _walkToTableInstance = Instantiate(walkToTable);
        _eatAtTableInstance = Instantiate(eating);
        _leaveInstance = Instantiate(leaving);

        _stateMachine = new StateMachine<CustomerState>();
    }

    void Start()
    {
        //if (npc == null) npc = GetComponent<NPC>();

        //npcOrderScript = GetComponentInChildren<NpcOrderScript>();
        //if (npcOrderScript == null)
        //{
        //    Debug.LogWarning($"{name}: NpcOrderScript not found in children!");
        //}

        //if (currentState == NPCState.WalkingToLine && targetLine != null)
        //    npc.MoveTo(targetLine.position);




        _orderingInstance.Initialize(gameObject, _stateMachine);
        _walkToTableInstance.Initialize(gameObject, _stateMachine);
        _eatAtTableInstance.Initialize(gameObject, _stateMachine);
        _leaveInstance.Initialize(gameObject, _stateMachine);

        _stateMachine.Initialize(_orderingInstance);
        _currentState = _stateMachine.GetCurrentState();
    }

    private void OnOrderReceived(MenuItem order)
    {
        if (!hasReceivedOrder && npcOrderScript != null)
        {
            npcOrderScript.SetFoodOrder(order);
            hasReceivedOrder = true;
            Debug.Log($"{name} received order: {order.GetFoodSprite().name}");
        }
    }
    
    void Update()
    {
        //switch (currentState)
        //{
        //    case NPCState.WalkingToLine: WalkToLine(); break;
        //    case NPCState.WaitingInLine: WaitInLine(); break;
        //    case NPCState.WalkingToTable: WalkToTable(); break;
        //    case NPCState.WaitingAtTable: WaitAtTable(); break;
        //    case NPCState.Leaving: Leave(); break;
        //}
        _stateMachine.RunUpdateLogic();
    }

    void FixedUpdate()
    {
        _stateMachine.RunFixedUpdateLogic();
    }

    public void OnInteract(GameObject player)
    {
        _stateMachine.GetCurrentState().InteractLogic(player);
    }

    void WalkToLine()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();
            transform.rotation = targetLine.rotation; 
            currentState = NPCState.WaitingInLine;
        }
    }
    void WaitInLine()   //replace with delivery condition here...
    {
        //waitTimer += Time.deltaTime;

        //if (waitTimer >= waitDuration)
        //{
        //    waitTimer = 0f;
        //    Debug.Log($"{name} done waiting in line, starting WalkToTable");
        //    assignedTable = manager.GetNextTable();
        //    currentState = NPCState.WalkingToTable;
        //}


    }
    void WalkToTable()
    {
       // npc.SetSpeechBubbleActive(false);
        //npc.MoveTo(assignedTable.position);

        // manager.RemoveCustomerFromLine(this);
        manager.RemoveCustomerFromLine();


        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();
            transform.rotation = assignedTable.rotation; 
            currentState = NPCState.WaitingAtTable;
            Debug.Log($"{name} reached table {assignedTable.name}, starting WaitAtTable");
        }
    }

    void WaitAtTable()
    {
        if (!isEating)
        {
            var currentOrder = npcOrderScript.GetFoodOrder();
            if (currentOrder != null)
            {
                switch (currentOrder.GetOrderType())
                {
                    case MenuItemType.Drink:
                        animator.SetBool("isDrinking", true);
                        Debug.Log($"{name} started drinking");
                        break;

                    default: 
                        animator.SetBool("isEating", true);
                        Debug.Log($"{name} started eating");
                        break;
                }
            }

            isEating = true;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (isEating && stateInfo.normalizedTime >= 0.95f && !animator.IsInTransition(0))
        {
            animator.SetBool("isEating", false);
            animator.SetBool("isDrinking", false);

            isEating = false;
            Debug.Log($"{name} finished eating/drinking, leaving now");

            leavePosition = manager.GetLeavePosition();
            currentState = NPCState.Leaving;
        }
    }

    void Leave()
    {
        //animator.SetBool("isEating", false);

        //if (heldFoodItem != null)
        //{
        //    Destroy(heldFoodItem);
        //}

        //npc.MoveTo(leavePosition.position);
        //if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        //{
        //    agent.ResetPath();
        //    Debug.Log($"{name} has left, destroying NPC object");
        //    Destroy(gameObject);
        //}
    }
    
    public MenuItem GetCurrentOrder()
    {
        return npcOrderScript != null ? npcOrderScript.GetFoodOrder() : null;
    }

    private void OnPlayerInteractedWithMe(GameObject player)
    {
        // is the player holding an object?
        //RagdollController rdController = player.GetComponent<RagdollController>();
        //GameObject leftHandGrabbedObj = rdController.leftHandGrabDetection.grabbedObj;
        //GameObject rightHandGrabbedObj = rdController.rightHandGrabDetection.grabbedObj;

        //if (leftHandGrabbedObj == null && rightHandGrabbedObj == null)
        //{
        //    Debug.Log($"{name}: Player is not holding any object.");
        //    return;
        //}

        //if (leftHandGrabbedObj != null)
        //{
        //    string leftHandGrabbedObjPrefabName = leftHandGrabbedObj.GetComponent<PrefabContainer>().GetPrefabName();
        //    if (leftHandGrabbedObjPrefabName == npcOrderScript.GetFoodOrder().GetOrderItemPrefab().name)
        //    {
        //        // Pass the player number instead of GameObject
        //        ScoreSystem.ChangeScore(545, player);
                
        //        GrabSystem.ReleaseObject(player.GetComponent<HandContainer>().LeftHand);
        //        ObjectPoolManager.ReturnObjectToPool(leftHandGrabbedObj);
        //        Debug.Log($"{name} received correct order from {player.name}!");
        //        SpawnFoodInHand();
        //        assignedTable = manager.GetNextTable();
        //        currentState = NPCState.WalkingToTable;
        //        return;
        //    }
        //    else
        //    {
        //        Debug.Log($"{name} received incorrect order from {player.name}.");
        //        return;
        //    }
        //}

        //if (rightHandGrabbedObj != null)
        //{
        //    string rightHandGrabbedObjPrefabName = rightHandGrabbedObj.GetComponent<PrefabContainer>().GetPrefabName();
        //    if (rightHandGrabbedObjPrefabName == npcOrderScript.GetFoodOrder().GetOrderItemPrefab().name)
        //    {
        //        Debug.Log($"{name} received correct order from {player.name}!");
                
        //        ScoreSystem.ChangeScore(545, player);
                
        //        GrabSystem.ReleaseObject(player.GetComponent<HandContainer>().RightHand);
        //        ObjectPoolManager.ReturnObjectToPool(rightHandGrabbedObj);
        //        SpawnFoodInHand();
        //        assignedTable = manager.GetNextTable();
        //        currentState = NPCState.WalkingToTable;
        //        return;
        //    }
        //    else
        //    {
        //        Debug.Log($"{name} received incorrect order from player.");
        //        return;
        //    }
        //}
    }

    private void SpawnFoodInHand()
    {
        if (handHoldPoint == null)
        {
            Debug.LogWarning($"{name}: handHoldPoint not assigned!");
            return;
        }

        MenuItem currentOrder = npcOrderScript.GetFoodOrder();
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

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void ChangeState(CustomerState newState)
    {
        _stateMachine.ChangeState(newState);
        _currentState = newState;
    }

}
