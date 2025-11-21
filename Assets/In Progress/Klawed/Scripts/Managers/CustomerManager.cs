using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CustomerManager : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private int maxVisibleSpeech = 4;

    [Header("Order Settings")]
    [SerializeField] private string assignedChannel = "OrderDisplay";

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxActiveCustomers = 4;

    [Header("References")]
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private Transform[] linePositions;
    [SerializeField] private Transform[] tablePositions;
    [SerializeField] private Transform[] leavePositions;

    [Header("Debug")]
    [ReadOnly]
    [SerializeField] private List<Customer> customerLine = new List<Customer>();
    private float spawnTimer = 0f;  //remove
    private int nextTableIndex = 0;

    private int numOfCustomers => customerLine.Count;

    private void Start()
    {
        GenericEvent<OnCustomerServed>.GetEvent("OnCustomerServed").AddListener(OnCustomerServedLogic);
        GenericEvent<CustomerFinishedEating>.GetEvent("CustomerFinishedEating").AddListener(OnCustomerFinishedEating);
    }



    private void Update()
    {

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            if (customerLine.Count < maxActiveCustomers)
            {
                SpawnCustomer();
                spawnTimer = 0f;
            }
        }

    }

    public void SpawnCustomer()
    {
        if (numOfCustomers >= maxActiveCustomers)
            return;

        Transform spawnPoint = spawnPositions[Random.Range(0, spawnPositions.Length)];

        if (NavMeshExists(spawnPoint.position))
        {
            GameObject customerObj = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);

            Customer customer = customerObj.GetComponent<Customer>();
            customerLine.Add(customer);
            AssignLinePosition(customer);
            AssignOrder(customer);
            UpdateSpeechBubbles();
        }
        else
        {
            Debug.LogWarning("No NavMesh found at spawn position: " + spawnPoint.position);
        }
    }

    private bool NavMeshExists(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 2.0f, NavMesh.AllAreas);
    }

    private void AssignOrder(Customer customer)
    {
        MenuItem order = OrderManager.Instance.GetRandomOrder();
        customer.SetOrder(order);
    }

    private void RemoveCustomerFromLine(Customer customer)
    {
        customerLine.Remove(customer);
        UpdateCustomerLinePositions();
    }

    private void AssignLinePosition(Customer customer)
    {
        int index = customerLine.IndexOf(customer);
        customer.targetLine = linePositions[index];
        customer.linePosition = index;
    }

    private void UpdateCustomerLinePositions()
    {
        for (int i = 0; i < customerLine.Count; i++)
        {
            Customer customer = customerLine[i];
            customer.targetLine = linePositions[i];
            customer.MoveTo(customer.targetLine.position);
            customer.linePosition = i;
        }
        UpdateSpeechBubbles();
    }

    private void UpdateSpeechBubbles()
    {
        for (int i = 0; i < customerLine.Count; i++)
        {
            bool showBubble = i < maxVisibleSpeech;
            //npcLine[i].npc.SetSpeechBubbleActive(showBubble);
        }
    }

    private void OnCustomerServedLogic(Customer customer)
    {
        AssignTableToCustomer(customer);
        RemoveCustomerFromLine(customer);
        UpdateCustomerLinePositions();

    }

    private void OnCustomerFinishedEating(Customer customer)
    {
        Transform leavePoint = GetLeavePosition();
        customer.exitPoint = leavePoint;
    }

    public Transform AssignTableToCustomer(Customer customer)
    {
        if (tablePositions.Length == 0) return null;

        Transform table = tablePositions[nextTableIndex];
        nextTableIndex = (nextTableIndex + 1) % tablePositions.Length;

        Debug.Log($"Next table: {table.name}");
        customer.assignedTable = table;
        return table;
    }

    public Transform GetLeavePosition()
    {
        Transform leavePoint = leavePositions[Random.Range(0, leavePositions.Length)];

        return leavePoint;
    }

}
