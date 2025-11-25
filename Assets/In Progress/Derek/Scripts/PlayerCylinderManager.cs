using UnityEngine;
using System.Collections.Generic;

public class PlayerCylinderManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> playerCylinders = new List<GameObject>();
    
    private void Start()
    {
        // Hide all cylinders at start
        HideAllCylinders();
        
        // Update cylinders based on current player count
        UpdateCylinderVisibility();
    }
    
    private void OnEnable()
    {
        // Listen for player joined events
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").AddListener(OnPlayerCountChanged);
    }
    
    private void OnDisable()
    {
        // Remove listener
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").RemoveListener(OnPlayerCountChanged);
    }
    
    private void OnPlayerCountChanged(GameObject player)
    {
        UpdateCylinderVisibility();
    }
    
    private void UpdateCylinderVisibility()
    {
        int playerCount = PlayerManager.Instance.PlayerCount;
        
        // Show cylinders up to the current player count
        for (int i = 0; i < playerCylinders.Count; i++)
        {
            if (playerCylinders[i] != null)
            {
                playerCylinders[i].SetActive(i < playerCount);
            }
        }
        
        Debug.Log($"Updated cylinder visibility for {playerCount} players");
    }
    
    private void HideAllCylinders()
    {
        foreach (var cylinder in playerCylinders)
        {
            if (cylinder != null)
            {
                cylinder.SetActive(false);
            }
        }
    }
    
    // Optional: Public method to manually update if needed
    public void RefreshCylinderVisibility()
    {
        UpdateCylinderVisibility();
    }
}