using UnityEngine;
using System.Collections.Generic;
using TMPro; // If using TextMeshPro (recommended)
// using UnityEngine.UI; // If using standard Unity Text

[System.Serializable]
public class PlayerCylinderData
{
    public GameObject cylinder;
    public TextMeshProUGUI playerText; // Or use Text if not using TextMeshPro
}

public class PlayerCylinderManager : MonoBehaviour
{
    [SerializeField] private List<PlayerCylinderData> playerCylinders = new List<PlayerCylinderData>();

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

        // Show cylinders and text up to the current player count
        for (int i = 0; i < playerCylinders.Count; i++)
        {
            bool shouldBeActive = i < playerCount;
            
            if (playerCylinders[i].cylinder != null)
            {
                playerCylinders[i].cylinder.SetActive(shouldBeActive);
            }
            
            if (playerCylinders[i].playerText != null)
            {
                playerCylinders[i].playerText.gameObject.SetActive(shouldBeActive);
            }
        }

        Debug.Log($"Updated cylinder visibility for {playerCount} players");
    }

    private void HideAllCylinders()
    {
        foreach (var cylinderData in playerCylinders)
        {
            if (cylinderData.cylinder != null)
            {
                cylinderData.cylinder.SetActive(false);
            }
            
            if (cylinderData.playerText != null)
            {
                cylinderData.playerText.gameObject.SetActive(false);
            }
        }
    }

    // Optional: Public method to manually update if needed
    public void RefreshCylinderVisibility()
    {
        UpdateCylinderVisibility();
    }
}