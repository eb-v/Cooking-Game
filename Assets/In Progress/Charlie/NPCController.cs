using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public NPCState currentState;  
    public NPC npc;

    [HideInInspector] public Transform targetLine;
    [HideInInspector] public Transform[] tablePositions;
    [HideInInspector] public Transform exitPoint;

    void Start()
    {
        if (npc == null) npc = GetComponent<NPC>();
        if (currentState == NPCState.WalkingToLine && targetLine != null)
            npc.MoveTo(targetLine.position);

        npc.agent.updateRotation = true;
        npc.agent.updatePosition = true;
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
        
    }
    void WalkToTable() { }

    void WaitAtTable() { }
    
    void Leave() { }
}
