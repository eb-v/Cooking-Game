using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public NPCState currentState;
    public NPC npc;
    public NpcManager manager;


    [HideInInspector] public Transform targetLine;
    [HideInInspector] public Transform tablePositions;
    [HideInInspector] public Transform exitPoint;

    private float waitTimer = 0f;  //remove
    private float waitDuration = 20f;  //remove
    private float leaveTimer = 0f;
    private float leaveDuration = 5f;
    private Transform assignedTable;
    private Transform leavePosition;

    void Start()
    {
        if (npc == null) npc = GetComponent<NPC>();
        if (currentState == NPCState.WalkingToLine && targetLine != null)
            npc.MoveTo(targetLine.position);
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
        waitTimer += Time.deltaTime;

        if (waitTimer >= waitDuration)
        {
            waitTimer = 0f;
            Debug.Log($"{name} done waiting in line, starting WalkToTable");
            assignedTable = manager.GetNextTable();
            currentState = NPCState.WalkingToTable;
        }
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
}
