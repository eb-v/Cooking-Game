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

    private List<NPC> npcLine = new List<NPC>();
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

        Vector3 spawnPos = linePositions[index].position;

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

        GameObject npcObj = Instantiate(npcPrefab, linePositions[index].position, linePositions[index].rotation);
        NPC spawnedNpc = npcObj.GetComponent<NPC>(); 
        npcLine.Add(spawnedNpc);

        spawnedNpc.MoveTo(spawnPos);

        UpdateSpeechBubbles();
    }

//call when npc leaves, need to change to moving away not destroying
    public void RemoveNpc(NPC npc)
    {
        if (!npcLine.Contains(npc)) return;

        npcLine.Remove(npc);
        Destroy(npc.gameObject); 

        for (int i = 0; i < npcLine.Count; i++)
        {
            npcLine[i].MoveTo(linePositions[i].position);
        }

        UpdateSpeechBubbles();
    }

    private void UpdateSpeechBubbles()
    {
        for (int i = 0; i < npcLine.Count; i++)
        {
            bool showBubble = i < maxVisibleSpeech;
            npcLine[i].SetSpeechBubbleActive(showBubble);
        }
    }
}
