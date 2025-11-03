using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;

    [SerializeField] private GameObject npcCanvas; // speech bubble

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool walking = agent.velocity.magnitude > 0.1f;
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
