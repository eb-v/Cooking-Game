using System;
using UnityEngine;
using UnityEngine.AI;

public class NPCHealth : MonoBehaviour {
    public int maxHealth = 100;
    private int currentHealth;

    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform player;


    [Header("Hit Indicator")]
    [SerializeField] private GameObject hitIndicatorPrefab;
    private GameObject currentIndicator;
    [SerializeField] private Transform indicatorPosition;
    void Start() {
        currentHealth = maxHealth;

        if (animator == null) animator = GetComponent<Animator>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int amount) {
        Debug.Log($"{gameObject.name} took {amount} damage.");

        currentHealth -= amount;


        ShowHitIndicator();

        RunAway();

        //if (currentHealth <= 0) {
        //    Die();
        //}
    }

    //private void Die() {
    //    isDead = true;
    //    animator.SetTrigger("Die");
    //    agent.isStopped = true;
    //    Destroy(gameObject, 4f);
    //}

    private void RunAway() {
        Debug.Log($"{gameObject.name} is running away!");

        // Play walk/run animation
        animator.Play("Amature|WalkCycle");
        animator.speed = 2.0f;

        agent.speed = 6f;

        Customer customer = GetComponent<Customer>();
        if (customer != null && customer.isRobber) {
            customer.LeaveImmediately();
            return;
        }

        Vector3 direction = transform.position - player.position;
        direction.y = 0;

        Vector3 runTarget = transform.position + direction.normalized * 10f;
        agent.SetDestination(runTarget);
    }

    private void OnTriggerEnter(Collider other) {
        Grabable gs = other.GetComponent<Grabable>()
                        ?? other.GetComponentInParent<Grabable>()
                        ?? other.GetComponentInChildren<Grabable>();

        if (gs != null) {
            Debug.Log($"{gameObject.name} hit by grabable object: {other.gameObject.name}");
            TakeDamage(50);
        }
    }

    private void ShowHitIndicator() {
        if (hitIndicatorPrefab == null || indicatorPosition == null) return;

        if (currentIndicator != null) Destroy(currentIndicator);

        currentIndicator = Instantiate(hitIndicatorPrefab, indicatorPosition.position, Quaternion.identity, indicatorPosition);

        Destroy(currentIndicator, 2f);
    }

}
