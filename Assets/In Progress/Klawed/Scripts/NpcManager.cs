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
    [SerializeField] private float spawnInterval = 5f;  //remove
    [SerializeField] private Transform[] spawnPositions;

    private List<NPCController> npcLine = new List<NPCController>();
    private float spawnTimer = 0f;  //remove
    private int nextTableIndex = 0;

    void Start()
    {
        GenericEvent<NewOrderAddedEvent>.GetEvent(assignedChannel).AddListener(OnNewOrderAdded);

        Debug.Log($"NpcManager listening on channel: {assignedChannel}");
    }

    void OnDestroy()
    {
        GenericEvent<NewOrderAddedEvent>.GetEvent(assignedChannel).RemoveListener(OnNewOrderAdded);
    }

    private void OnNewOrderAdded(FoodOrder order)
    {
        SpawnNpc();
    }

/*
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnNpc();
        }
    }
*/

    public void SpawnNpc()
    {
        int index = npcLine.Count;
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
        NPCController npcController = npcObj.GetComponent<NPCController>();
        npcController.manager = this;

        npcController.targetLine = linePositions[index];
        npcController.tablePositions = tablePositions[index];
        npcController.currentState = NPCState.WalkingToLine;

        npcLine.Add(npcController);
        UpdateSpeechBubbles();
    }

    public void RemoveNpc(NPCController npc)
    {
        if (!npcLine.Contains(npc)) return;

        npcLine.Remove(npc); 

        for (int i = 0; i < npcLine.Count; i++)
        {
            npcLine[i].targetLine = linePositions[i];
            npcLine[i].npc.MoveTo(linePositions[i].position);
            Debug.Log($"{npcLine[i].name} moved up to line position {linePositions[i].position}");
        }

        UpdateSpeechBubbles();
        SpawnNpc();
    }

    private void UpdateSpeechBubbles()
    {
        for (int i = 0; i < npcLine.Count; i++)
        {
            bool showBubble = i < maxVisibleSpeech;
            npcLine[i].npc.SetSpeechBubbleActive(showBubble);
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
