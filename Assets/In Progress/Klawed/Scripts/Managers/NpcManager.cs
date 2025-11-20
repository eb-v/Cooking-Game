using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform[] linePositions;
    [SerializeField] private Transform[] tablePositions;
    [SerializeField] private Transform[] leavePositions;
    [SerializeField] private int maxVisibleSpeech = 4;

    [Header("Order Settings")]
    [SerializeField] private string assignedChannel = "OrderDisplay";

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private Transform[] spawnPositions;

    [Header("References")]
    [SerializeField] private OrderSystem orderDeliveryManager;

    private Queue<Customer> customerLine = new Queue<Customer>();
    private float spawnTimer = 0f;  //remove
    private int nextTableIndex = 0;

    private void Start()
    {
        GenericEvent<OnCustomerServed>.GetEvent("OnCustomerServed").AddListener(RemoveCustomerFromLine);
    }



    private void Update()
    {

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnCustomer();
            spawnTimer = 0f;
        }

    }

    public void SpawnCustomer()
    {
        int index = customerLine.Count;
        if (index >= linePositions.Length || index >= tablePositions.Length) return;

        Transform spawnPoint = spawnPositions[Random.Range(0, spawnPositions.Length)];
        Vector3 spawnPos = spawnPoint.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 2f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }
        else
        {
            Debug.LogWarning($"No NavMesh found near spawn position {spawnPos}. NPC not spawned.");
            return;
        }

        GameObject npcObj = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
        
        Customer customer = npcObj.GetComponent<Customer>();
        customer.manager = this;

        InitializeNpcOrder(npcObj);

        customer.targetLine = linePositions[index];
        customer.tablePositions = tablePositions[index];
        customer.currentState = NPCState.WalkingToLine;

        customerLine.Enqueue(customer);
        UpdateSpeechBubbles();
    }

    private void InitializeNpcOrder(GameObject npc)
    {
        MenuItem order = orderDeliveryManager.GetRandomOrder();
        NpcOrderScript npcOrderScript = npc.GetComponent<NpcOrderScript>();
        npcOrderScript.SetFoodOrder(order);
    }

    // private void RemoveCustomerFromLine()
    // {
    //     customerLine.Dequeue();
    //     UpdateCustomerLinePositions();
    // }

    public void RemoveCustomerFromLine()
{
    customerLine.Dequeue();
    UpdateCustomerLinePositions();
}


    private void UpdateCustomerLinePositions()
    {
        Customer[] customers = customerLine.ToArray();

        for (int i = 0; i < customers.Length; i++)
        {
            customers[i].targetLine = linePositions[i];
            customers[i].MoveTo(customers[i].targetLine.position);
            customers[i].transform.rotation = linePositions[i].rotation;
        }
    }

    private void UpdateSpeechBubbles()
    {
        for (int i = 0; i < customerLine.Count; i++)
        {
            bool showBubble = i < maxVisibleSpeech;
            //npcLine[i].npc.SetSpeechBubbleActive(showBubble);
        }
    }

    public Transform GetNextTable()
    {
        if (tablePositions.Length == 0) return null;

        Transform table = tablePositions[nextTableIndex];
        nextTableIndex = (nextTableIndex + 1) % tablePositions.Length;

        Debug.Log($"Next table: {table.name}");
        return table;
    }

    public Transform GetLeavePosition()
    {
        Transform leavePoint = leavePositions[Random.Range(0, leavePositions.Length)];

        return leavePoint;
    }

}
