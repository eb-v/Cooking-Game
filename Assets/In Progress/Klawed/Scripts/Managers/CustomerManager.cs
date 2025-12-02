using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomerManager : MonoBehaviour {
    [Header("NPC Settings")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private int maxVisibleSpeech = 4;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxActiveCustomers = 4;

    [Header("References")]
    [SerializeField] public Transform[] spawnPositions;
    [SerializeField] private Transform[] linePositions;
    [SerializeField] private Transform[] tablePositions;
    [SerializeField] private Transform[] leavePositions;

    [Header("Debug")]
    [ReadOnly][SerializeField] private List<Customer> customerLine = new List<Customer>();

    public static CustomerManager Instance;


    private float spawnTimer = 0f;
    private int nextTableIndex = 0;

    private int numOfCustomers => customerLine.Count;


    [Header("Special NPCs")]
    [SerializeField] private GameObject robberPrefab;

    [Header("Robber Settings")]
    [SerializeField] private int robberScorePenalty = 1;
    [SerializeField] private float penaltyInterval = 1f;
    private float penaltyTimer = 0f;


    private void Start() {
        GenericEvent<OnCustomerServed>.GetEvent("OnCustomerServed").AddListener(OnCustomerServedLogic);
        GenericEvent<CustomerFinishedEating>.GetEvent("CustomerFinishedEating").AddListener(OnCustomerFinishedEating);
    }

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        spawnTimer += Time.deltaTime;
        penaltyTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval) {
            if (customerLine.Count < maxActiveCustomers) {
                SpawnCustomer();
            }
            spawnTimer = 0f;
        }

        if (penaltyTimer >= penaltyInterval) {
            foreach (Customer customer in customerLine) {
                if (customer.isRobber) {
                    ScoreManager.Instance.ChangeScore(-robberScorePenalty, null);
                }
            }
            penaltyTimer = 0f;
        }
    }

    public void SpawnCustomer() {
        if (numOfCustomers >= maxActiveCustomers)
            return;

        Transform spawnPoint = spawnPositions[Random.Range(0, spawnPositions.Length)];

        if (NavMeshExists(spawnPoint.position)) {
            GameObject customerObj = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);

            Customer customer = customerObj.GetComponent<Customer>();
            if (customer != null) {
                customerLine.Add(customer);
                AssignLinePosition(customer);
                AssignOrder(customer);
            }

            UpdateCustomerLinePositions();
        } else {
            Debug.LogWarning("No NavMesh found at spawn position: " + spawnPoint.position);
        }
    }

    public void SpawnRobber() {
        if (numOfCustomers >= maxActiveCustomers)
            return;

        Transform spawnPoint = spawnPositions[Random.Range(0, spawnPositions.Length)];

        if (NavMeshExists(spawnPoint.position)) {
            GameObject robberObj = Instantiate(robberPrefab, spawnPoint.position, spawnPoint.rotation);

            Customer robber = robberObj.GetComponent<Customer>();
            if (robber != null) {
                robber.isRobber = true;

                if (robber.Agent != null) {
                    robber.Agent.speed = 6f;
                }
                if (robber.Animator != null) {
                    robber.Animator.speed = 1.5f;
                }

                customerLine.Insert(0, robber);
                UpdateCustomerLinePositions();
            }
        } else {
            Debug.LogWarning("No NavMesh found at spawn position for robber: " + spawnPoint.position);
        }
    }


    private bool NavMeshExists(Vector3 position) {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 2.0f, NavMesh.AllAreas);
    }

    private void AssignOrder(Customer customer) {
        if (customer.isRobber) {
            customer.SetOrder(null);
            return;
        }

        if (OrderManager.Instance == null) {
            Debug.LogError("OrderManager instance not found. Cannot assign order to customer.");
            return;
        }

        MenuItem order = OrderManager.Instance.GetRandomOrder();
        customer.SetOrder(order);
    }


    private void RemoveCustomerFromLine(Customer customer) {
        customerLine.Remove(customer);
        UpdateCustomerLinePositions();
    }

    private void AssignLinePosition(Customer customer) {
        int index = customerLine.IndexOf(customer);
        customer.targetLine = linePositions[index];
        customer.linePosition = index;
    }

    private void UpdateCustomerLinePositions() {
        for (int i = 0; i < customerLine.Count; i++) {
            Customer customer = customerLine[i];
            customer.targetLine = linePositions[i];
            customer.MoveTo(customer.targetLine.position);
            customer.linePosition = i;

            if (!customer.isRobber && customer.IsInLine() && i < maxVisibleSpeech) {
                customer.DisplayImage(true);
            } else {
                customer.DisplayImage(false);
            }
        }
    }

    private void OnCustomerServedLogic(Customer customer) {
        AssignTableToCustomer(customer);
        RemoveCustomerFromLine(customer);
        UpdateCustomerLinePositions();
    }

    private void OnCustomerFinishedEating(Customer customer) {
        Transform leavePoint = GetLeavePosition();
        customer.exitPoint = leavePoint;
    }

    public Transform AssignTableToCustomer(Customer customer) {
        if (tablePositions.Length == 0) return null;

        Transform table = tablePositions[nextTableIndex];
        nextTableIndex = (nextTableIndex + 1) % tablePositions.Length;

        customer.assignedTable = table;
        return table;
    }

    public Transform GetLeavePosition() {
        return leavePositions[Random.Range(0, leavePositions.Length)];
    }
}
