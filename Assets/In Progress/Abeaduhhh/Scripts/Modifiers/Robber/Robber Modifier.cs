using UnityEngine;

public class RobberModifier : MonoBehaviour {
    [Header("Robber Settings")]
    public CustomerManager customerManager;
    public float spawnInterval = 30f;
    public int maxRobbers = 5;

    private int robbersSpawned = 0;
    private float timer = 0f;

    private void Update() {
        if (robbersSpawned >= maxRobbers) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval) {
            SpawnRobber();
            timer = 0f;
        }
    }

    private void SpawnRobber() {
        if (customerManager == null) {
            Debug.LogWarning("CustomerManager not assigned!");
            return;
        }

        customerManager.SpawnRobber();
        robbersSpawned++;
        Debug.Log("Robber spawned!");
    }

    private void OnDrawGizmosSelected() {
        if (customerManager == null) return;

        Gizmos.color = Color.red;

        // Optionally, draw spawn points
        foreach (var pos in customerManager.spawnPositions) {
            if (pos != null)
                Gizmos.DrawSphere(pos.position, 0.5f);
        }
    }
}
