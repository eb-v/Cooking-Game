using UnityEngine;

public class IdleNPC : MonoBehaviour
{
    private Animator animator;
    [SerializeField] bool npcIdle = true;
    [SerializeField] bool npcWalking = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            if (npcIdle)
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
            if (npcWalking)
                animator.SetBool("isWalking", true);
                animator.SetBool("isIdle", false);
        }
    }
}