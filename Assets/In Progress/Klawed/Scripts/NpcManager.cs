using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform[] linePositions;
    [SerializeField] private int maxVisibleSpeech = 4;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private Transform[] spawnPositions;

    private List<NPCController> npcLine = new List<NPCController>();
    private float spawnTimer = 0f;

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnNpc();
        }
    }

    public void SpawnNpc()
    {
        int index = npcLine.Count;
        if (index >= linePositions.Length) return;

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

        npcController.targetLine = linePositions[index];
        npcController.currentState = NPCState.WalkingToLine;

        npcLine.Add(npcController);
        UpdateSpeechBubbles();
    }

//call when npc leaves, need to change to moving away not destroying
    public void RemoveNpc(NPCController npc)
    {
        if (!npcLine.Contains(npc)) return;

        npcLine.Remove(npc);
        Destroy(npc.gameObject);

        // shift others up the line
        for (int i = 0; i < npcLine.Count; i++)
        {
            npcLine[i].targetLine = linePositions[i];
            npcLine[i].npc.MoveTo(linePositions[i].position);
        }

        UpdateSpeechBubbles();
    }

    private void UpdateSpeechBubbles()
    {
        for (int i = 0; i < npcLine.Count; i++)
        {
            bool showBubble = i < maxVisibleSpeech;
            npcLine[i].npc.SetSpeechBubbleActive(showBubble);
        }
    }
}
