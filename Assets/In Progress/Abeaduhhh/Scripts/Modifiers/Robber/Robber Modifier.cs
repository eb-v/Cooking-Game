using UnityEngine;

public class RobberModifier : MonoBehaviour {
    [Header("Robber Settings")]
    public CustomerManager customerManager;
    public int maxRobbers = 5;

    private int robbersSpawned = 0;

    public void SpawnRobber() {
        if (robbersSpawned >= maxRobbers) {
            Debug.Log("Maximum robbers spawned already.");
            return;
        }

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

        foreach (var pos in customerManager.spawnPositions) {
            if (pos != null)
                Gizmos.DrawSphere(pos.position, 0.5f);
        }
    }
}
