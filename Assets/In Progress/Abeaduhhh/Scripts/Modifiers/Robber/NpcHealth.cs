using UnityEngine;
using UnityEngine.AI;

public class NPCHealth : MonoBehaviour {
    public int maxHealth = 100;
    private int currentHealth;

    private bool isDead = false;

    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform player;

    void Start() {
        currentHealth = maxHealth;

        if (animator == null) animator = GetComponent<Animator>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int amount) {
        if (isDead) return;

        currentHealth -= amount;
        //if (currentHealth <= 0) {
        //    Die();
        //} else {
        //    RunAway();
        //}
    }

    //private void Die() {
    //    isDead = true;
    //    animator.SetTrigger("Die");
    //    agent.isStopped = true;
    //    // Optionally destroy after animation
    //    Destroy(gameObject, 5f);
    //}

    //private void RunAway() {
    //    animator.SetTrigger("Run");
    //    Vector3 runDirection = transform.position - player.position;
    //    runDirection.y = 0;
    //    Vector3 runTarget = transform.position + runDirection.normalized * 10f;
    //    agent.SetDestination(runTarget);
    //}
}
