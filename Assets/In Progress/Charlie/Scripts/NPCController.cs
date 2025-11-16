using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public NPCState currentState;
    public NPC npc;
    public NpcManager manager;

    [Header("Order Settings")]
    [SerializeField] private string assignedChannel;
    [SerializeField] private Transform handHoldPoint;

    private NpcOrderScript npcOrderScript;


    [HideInInspector] public Transform targetLine;
    [HideInInspector] public Transform tablePositions;
    [HideInInspector] public Transform exitPoint;

    private Transform assignedTable;
    private Transform leavePosition;
    private bool hasReceivedOrder = false;
    private bool isEating = false;
    private GameObject heldFoodItem;

    void Start()
    {
        if (npc == null) npc = GetComponent<NPC>();

        npcOrderScript = GetComponentInChildren<NpcOrderScript>();
        if (npcOrderScript == null)
        {
            Debug.LogWarning($"{name}: NpcOrderScript not found in children!");
        }

        GenericEvent<NewOrderAddedEvent>.GetEvent(assignedChannel).AddListener(OnOrderReceived);
        GenericEvent<OnCustomerInteract>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnPlayerInteractedWithMe);

        if (currentState == NPCState.WalkingToLine && targetLine != null)
            npc.MoveTo(targetLine.position);
    }

    void OnDestroy()
    {
        GenericEvent<NewOrderAddedEvent>.GetEvent(assignedChannel).RemoveListener(OnOrderReceived);
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
        switch (currentState)
        {
            case NPCState.WalkingToLine: WalkToLine(); break;
            case NPCState.WaitingInLine: WaitInLine(); break;
            case NPCState.WalkingToTable: WalkToTable(); break;
            case NPCState.WaitingAtTable: WaitAtTable(); break;
            case NPCState.Leaving: Leave(); break;
        }
    }

    void WalkToLine()
    {
        if (!npc.agent.pathPending && npc.agent.remainingDistance <= npc.agent.stoppingDistance)
        {
            npc.agent.ResetPath();
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
        npc.SetSpeechBubbleActive(false);
        npc.MoveTo(assignedTable.position);

        manager.RemoveNpc(this);

        if (!npc.agent.pathPending && npc.agent.remainingDistance <= npc.agent.stoppingDistance)
        {
            npc.agent.ResetPath();
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
                        npc.animator.SetBool("isDrinking", true);
                        Debug.Log($"{name} started drinking");
                        break;

                    default: 
                        npc.animator.SetBool("isEating", true);
                        Debug.Log($"{name} started eating");
                        break;
                }
            }

            isEating = true;
        }

        AnimatorStateInfo stateInfo = npc.animator.GetCurrentAnimatorStateInfo(0);

        if (isEating && stateInfo.normalizedTime >= 0.95f && !npc.animator.IsInTransition(0))
        {
            npc.animator.SetBool("isEating", false);
            npc.animator.SetBool("isDrinking", false);

            isEating = false;
            Debug.Log($"{name} finished eating/drinking, leaving now");

            leavePosition = manager.GetLeavePosition();
            currentState = NPCState.Leaving;
        }
    }

    void Leave()
    {
        npc.animator.SetBool("isEating", false);

        if (heldFoodItem != null)
        {
            Destroy(heldFoodItem);
        }

        npc.MoveTo(leavePosition.position);
        if (!npc.agent.pathPending && npc.agent.remainingDistance <= npc.agent.stoppingDistance)
        {
            npc.agent.ResetPath();
            Debug.Log($"{name} has left, destroying NPC object");
            Destroy(gameObject);
        }
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
}
