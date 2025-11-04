using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;

    [SerializeField] private GameObject npcCanvas; // speech bubble

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool walking = agent.pathPending || agent.remainingDistance > agent.stoppingDistance;
        animator.SetBool("isWalking", walking);

    }

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    //speech bubble visibility
    public void SetSpeechBubbleActive(bool active)
    {
        if (npcCanvas != null)
            npcCanvas.SetActive(active);
    }
}
