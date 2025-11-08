using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public NPCState currentState;
    public NPC npc;
    public NpcManager manager;

    [Header("Order Settings")]
    [SerializeField] private string assignedChannel;
    private NpcOrderScript npcOrderScript;


    [HideInInspector] public Transform targetLine;
    [HideInInspector] public Transform tablePositions;
    [HideInInspector] public Transform exitPoint;

    private float waitTimer = 0f;  //remove
    private float waitDuration = 20f;  //remove
    private float leaveTimer = 0f;
    private float leaveDuration = 5f;
    private Transform assignedTable;
    private Transform leavePosition;
    private bool hasReceivedOrder = false;

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
            currentState = NPCState.WaitingInLine;
            Debug.Log($"{name} reached line position, switching to WaitingInLine");
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
            leaveTimer = 0f;
            currentState = NPCState.WaitingAtTable;
            Debug.Log($"{name} reached table {assignedTable.name}, starting WaitAtTable");
        }
    }

    void WaitAtTable()
    {
        leaveTimer += Time.deltaTime;

        if (leaveTimer >= leaveDuration)
        {
            leaveTimer = 0f;
            Debug.Log($"{name} done waiting at table, starting Leaving");
            leavePosition = manager.GetLeavePosition();
            currentState = NPCState.Leaving;
        }
    }

    void Leave()
    {
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
        RagdollController rdController = player.GetComponent<RagdollController>();

        GameObject leftHandGrabbedObj = rdController.leftHandGrabDetection.grabbedObj;
        GameObject rightHandGrabbedObj = rdController.rightHandGrabDetection.grabbedObj;
        if (leftHandGrabbedObj == null && rightHandGrabbedObj == null)
        {
            Debug.Log($"{name}: Player is not holding any object.");
            return;
        }

        if (leftHandGrabbedObj != null)
        {
            string leftHandGrabbedObjPrefabName = leftHandGrabbedObj.GetComponent<PrefabContainer>().GetPrefabName();
            if (leftHandGrabbedObjPrefabName == npcOrderScript.GetFoodOrder().GetOrderItemPrefab().name)
            {
                GenericEvent<NpcReceivedCorrectOrder>.GetEvent("OrderSystem").Invoke(player);
                GrabSystem.ReleaseObject(player.GetComponent<HandContainer>().LeftHand);
                ObjectPoolManager.ReturnObjectToPool(leftHandGrabbedObj);
                Debug.Log($"{name} received correct order from player!");
                assignedTable = manager.GetNextTable();
                currentState = NPCState.WalkingToTable;
                return;
            }
            else
            {
                Debug.Log($"{name} received incorrect order from player.");
                // maybe play a sad sound or animation here
                return;
            }
        }

        if (rightHandGrabbedObj != null)
        {
            string rightHandGrabbedObjPrefabName = rightHandGrabbedObj.GetComponent<PrefabContainer>().GetPrefabName();
            if (rightHandGrabbedObjPrefabName == npcOrderScript.GetFoodOrder().GetOrderItemPrefab().name)
            {
                Debug.Log($"{name} received correct order from player!");
                GrabSystem.ReleaseObject(player.GetComponent<HandContainer>().RightHand);
                ObjectPoolManager.ReturnObjectToPool(rightHandGrabbedObj);
                assignedTable = manager.GetNextTable();
                currentState = NPCState.WalkingToTable;
                return;
            }
            else
            {
                Debug.Log($"{name} received incorrect order from player.");
                // maybe play a sad sound or animation here
                return;
            }
        }
    }
}
