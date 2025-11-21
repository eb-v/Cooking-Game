using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    [SerializeField] private GameObject npcCanvas; // speech bubble

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator component not found on NPC GameObject.");

        if (agent == null)
            Debug.LogError("NavMeshAgent component not found on NPC GameObject.");
    }

    void Start()
    {
        agent.updateRotation = true;
    }

    void Update()
    {
        bool walking = agent.pathPending || agent.remainingDistance > agent.stoppingDistance;
        animator.SetBool("isWalking", walking);

    }
}
