using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public NPCState currentState;  
    public NavMeshAgent agent;
    public Animator animator;
    
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

    void WalkToLine() { }
    void WaitInLine() {  }
    void WalkToTable() { }

    void WaitAtTable() { }
    
    void Leave() { }
}
