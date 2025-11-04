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

    private float waitTimer = 0f;
    private float waitDuration = 10f; 

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
        }
    }
    void WaitInLine()
    {
        waitTimer += Time.deltaTime;

        if (waitTimer >= waitDuration)
        {
            waitTimer = 0f;
            WalkToTable();
        }
    }
    void WalkToTable()
    {
        Transform table = manager.GetRandomTable();
        tablePositions = table;

        currentState = NPCState.WalkingToTable;
        npc.SetSpeechBubbleActive(false);
        npc.MoveTo(table.position);

        manager.RemoveNpc(this);
    }

    void WaitAtTable() { }
    
    void Leave() { }
}
