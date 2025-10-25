using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndGameAwards : MonoBehaviour
{
    [Header("Award Display")]
    [SerializeField] private GameObject awardDisplayPanel;
    [SerializeField] private Transform playerDisplayPosition;
    [SerializeField] private TextMeshProUGUI awardTitleText;
    [SerializeField] private TextMeshProUGUI awardDescriptionText;
    [SerializeField] private TextMeshProUGUI playerNamesText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private float displayDuration = 4f;
    
    [Header("Camera")]
    [SerializeField] private Camera awardCamera;
    [SerializeField] private float rotationSpeed = 30f;
    
    private List<GameObject> currentPlayerModels = new List<GameObject>();
    private bool ceremonyCancelled = false;
    
    public void ShowAwards()
    {
        // DEBUG: Check all players before starting ceremony
        Debug.Log("=== PRE-CEREMONY DEBUG ===");
        List<PlayerStats> debugPlayers = PlayerManager.Instance.GetAllPlayers();
        Debug.Log($"Total players found: {debugPlayers.Count}");
        foreach (var player in debugPlayers)
        {
            if (player != null)
            {
                Debug.Log($"Player {player.playerNumber} - Points: {player.pointsGenerated}, Ingredients: {player.ingredientsHandled}, Joints: {player.jointsReconnected}, Explosions: {player.explosionsReceived}");
            }
            else
            {
                Debug.LogWarning("Found NULL player in list!");
            }
        }
        Debug.Log("=== END PRE-CEREMONY DEBUG ===");
        
        StartCoroutine(AwardCeremony());
    }
    
    public void CancelCeremony()
    {
        ceremonyCancelled = true;
        StopAllCoroutines();
        ClearPlayerModels();
        awardDisplayPanel.SetActive(false);
    }
    
    private IEnumerator AwardCeremony()
    {
        ceremonyCancelled = false;
        
        // Turn on the award display panel (which is also the end game canvas)
        awardDisplayPanel.SetActive(true);
        
        Debug.Log("Starting Award Ceremony - Looping Mode");
    
        
        // Loop the ceremony indefinitely until cancelled
        while (!ceremonyCancelled)
        {
            // Hot Hands Award
            yield return StartCoroutine(ShowAward(
                "Hot Hands",
                "Most Ingredients Handled",
                GetHotHandsWinners(),
                (player) => $"Ingredients: {player.ingredientsHandled}"
            ));
            
            if (ceremonyCancelled) yield break;
            
            // MVP Award
            yield return StartCoroutine(ShowAward(
                "MVP",
                "Most Points Generated",
                GetMVPWinners(),
                (player) => $"Points: {player.pointsGenerated}"
            ));
            
            if (ceremonyCancelled) yield break;
            
            // Guardian Angel Award (only if someone reconnected joints)
            var guardianWinners = GetGuardianAngelWinners();
            if (guardianWinners.Count > 0)
            {
                yield return StartCoroutine(ShowAward(
                    "Guardian Angel",
                    "Most Joints Reconnected",
                    guardianWinners,
                    (player) => $"Joints: {player.jointsReconnected}"
                ));
            }
            
            if (ceremonyCancelled) yield break;
            
            // Noob Award
            yield return StartCoroutine(ShowAward(
                "Noob",
                "Worst Performance (Low Points + High Explosions)",
                GetNoobWinners(),
                (player) => $"Points: {player.pointsGenerated} | Explosions: {player.explosionsReceived}"
            ));
            
            if (ceremonyCancelled) yield break;
            
            Debug.Log("Award Ceremony Loop Complete - Restarting...");
            
            // Small pause before looping again
            yield return new WaitForSecondsRealtime(1f);
        }
        
        Debug.Log("Award Ceremony Ended");
    }
    
    private IEnumerator ShowAward(string title, string description, List<PlayerStats> winners, System.Func<PlayerStats, string> getStatText)
    {
        // Clear previous player models
        ClearPlayerModels();
        
        if (winners == null || winners.Count == 0)
        {
            Debug.Log($"Skipping award: {title} - No winners");
            yield break;
        }
        
        Debug.Log($"Showing award: {title}");
        
        // Set award description
        if (awardDescriptionText != null)
            awardDescriptionText.text = description;
        
        // Display player names
        string playerNames = "";
        string stats = "";
        for (int i = 0; i < winners.Count; i++)
        {
            playerNames += "Player " + winners[i].playerNumber;
            if (i < winners.Count - 1)
                playerNames += " & ";
            
            stats += getStatText(winners[i]);
            if (i < winners.Count - 1)
                stats += " | ";
        }
        
        // Add 's' to title if multiple winners (except for "Hot Hands" which already ends in 's')
        string displayTitle = title;
        if (winners.Count > 1 && title != "Hot Hands")
        {
            displayTitle += "s";
        }
        
        if (playerNamesText != null)
            playerNamesText.text = playerNames;
        
        if (awardTitleText != null)
            awardTitleText.text = displayTitle;
        
        if (statsText != null)
        {
            statsText.text = stats;
        }
        
        // Instantiate visual-only player models
        float spacing = 3f;
        float startX = -(winners.Count - 1) * spacing / 2f;
        
        for (int i = 0; i < winners.Count; i++)
        {
            // Create visual-only copy
            GameObject playerModel = CreateVisualOnlyCopy(
                winners[i].gameObject,
                playerDisplayPosition.position + new Vector3(startX + i * spacing, 0, 0),
                Quaternion.identity
            );
            
            playerModel.transform.SetParent(playerDisplayPosition);
            currentPlayerModels.Add(playerModel);
        }
        
        // Rotate camera around players for the duration
        float elapsed = 0f;
        Vector3 centerPosition = playerDisplayPosition.position;
        
        while (elapsed < displayDuration && !ceremonyCancelled)
        {
            elapsed += Time.unscaledDeltaTime;
            
            // Rotate camera around the display position
            if (awardCamera != null)
            {
                awardCamera.transform.RotateAround(
                    centerPosition,
                    Vector3.up,
                    rotationSpeed * Time.unscaledDeltaTime
                );
                
                // Keep camera looking at the center
                awardCamera.transform.LookAt(centerPosition);
            }
            
            yield return null;
        }
        
        yield return new WaitForSecondsRealtime(0.5f);
    }
    
    // ==================== AWARD WINNER CALCULATION METHODS ====================
    
    private List<PlayerStats> GetHotHandsWinners()
    {
        List<PlayerStats> allPlayers = PlayerManager.Instance.GetAllPlayers();
        
        if (allPlayers.Count == 0)
        {
            Debug.LogWarning("No players found for Hot Hands award");
            return new List<PlayerStats>();
        }
        
        // Debug all player stats
        Debug.Log("=== Hot Hands Award ===");
        foreach (var player in allPlayers)
        {
            Debug.Log($"Player {player.playerNumber}: {player.ingredientsHandled} ingredients");
        }
        
        int maxIngredients = 0;
        foreach (var player in allPlayers)
        {
            if (player != null && player.ingredientsHandled > maxIngredients)
                maxIngredients = player.ingredientsHandled;
        }
        
        List<PlayerStats> winners = new List<PlayerStats>();
        foreach (var player in allPlayers)
        {
            if (player != null && player.ingredientsHandled == maxIngredients)
                winners.Add(player);
        }
        
        Debug.Log($"Hot Hands Winner(s): {winners.Count} player(s) with {maxIngredients} ingredients");
        return winners;
    }
    
    private List<PlayerStats> GetMVPWinners()
    {
        List<PlayerStats> allPlayers = PlayerManager.Instance.GetAllPlayers();
        
        if (allPlayers.Count == 0)
        {
            Debug.LogWarning("No players found for MVP award");
            return new List<PlayerStats>();
        }
        
        // Debug all player stats
        Debug.Log("=== MVP Award ===");
        foreach (var player in allPlayers)
        {
            Debug.Log($"Player {player.playerNumber}: {player.pointsGenerated} points");
        }
        
        int maxPoints = 0;
        foreach (var player in allPlayers)
        {
            if (player != null && player.pointsGenerated > maxPoints)
                maxPoints = player.pointsGenerated;
        }
        
        // Skip award if no one scored any points
        if (maxPoints == 0)
        {
            Debug.Log("No points scored this game - skipping MVP award");
            return new List<PlayerStats>();
        }
        
        List<PlayerStats> winners = new List<PlayerStats>();
        foreach (var player in allPlayers)
        {
            if (player != null && player.pointsGenerated == maxPoints)
                winners.Add(player);
        }
        
        Debug.Log($"MVP Winner(s): {winners.Count} player(s) with {maxPoints} points");
        return winners;
    }
    
    private List<PlayerStats> GetGuardianAngelWinners()
    {
        List<PlayerStats> allPlayers = PlayerManager.Instance.GetAllPlayers();
        
        if (allPlayers.Count == 0) return new List<PlayerStats>();
        
        int maxReconnections = 0;
        foreach (var player in allPlayers)
        {
            if (player != null && player.jointsReconnected > maxReconnections)
                maxReconnections = player.jointsReconnected;
        }
        
        if (maxReconnections == 0)
        {
            Debug.Log("No joint reconnections this game - skipping Guardian Angel award");
            return new List<PlayerStats>();
        }
        
        // Debug all player stats
        Debug.Log("=== Guardian Angel Award ===");
        foreach (var player in allPlayers)
        {
            Debug.Log($"Player {player.playerNumber}: {player.jointsReconnected} joints reconnected");
        }
        
        List<PlayerStats> winners = new List<PlayerStats>();
        foreach (var player in allPlayers)
        {
            if (player != null && player.jointsReconnected == maxReconnections)
                winners.Add(player);
        }
        
        Debug.Log($"Guardian Angel Winner(s): {winners.Count} player(s) with {maxReconnections} reconnections");
        return winners;
    }
    
    private List<PlayerStats> GetNoobWinners()
    {
        List<PlayerStats> allPlayers = PlayerManager.Instance.GetAllPlayers();
        
        if (allPlayers.Count == 0)
        {
            Debug.LogWarning("No players found for Noob award");
            return new List<PlayerStats>();
        }
        
        // Debug all player stats
        Debug.Log("=== Noob Award ===");
        foreach (var player in allPlayers)
        {
            Debug.Log($"Player {player.playerNumber}: {player.pointsGenerated} points, {player.explosionsReceived} explosions");
        }
        
        // Calculate "noob score" - combination of low points and high explosions
        // Lower points = worse, more explosions = worse
        // Formula: points - (explosions * weight)
        // The LOWEST score wins the noob award
        float worstScore = float.MaxValue;
        int explosionWeight = 10; // Each explosion is worth -10 points in the noob score
        
        foreach (var player in allPlayers)
        {
            if (player != null)
            {
                float noobScore = player.pointsGenerated - (player.explosionsReceived * explosionWeight);
                if (noobScore < worstScore)
                    worstScore = noobScore;
            }
        }
        
        List<PlayerStats> winners = new List<PlayerStats>();
        foreach (var player in allPlayers)
        {
            if (player != null)
            {
                float noobScore = player.pointsGenerated - (player.explosionsReceived * explosionWeight);
                if (Mathf.Approximately(noobScore, worstScore))
                    winners.Add(player);
            }
        }
        
        Debug.Log($"Noob Winner(s): {winners.Count} player(s) with worst score of {worstScore}");
        return winners;
    }
    
    // ==================== VISUAL COPY METHODS ====================
    
    private GameObject CreateVisualOnlyCopy(GameObject original, Vector3 position, Quaternion rotation)
    {
        // Create a new empty GameObject
        GameObject copy = new GameObject($"AwardDisplay_{original.name}");
        copy.transform.position = position;
        copy.transform.rotation = rotation;
        
        // Copy the transform hierarchy and visual components recursively
        CopyVisualComponents(original.transform, copy.transform);
        
        return copy;
    }
    
    private void CopyVisualComponents(Transform source, Transform destination)
    {
        // Copy MeshFilter
        MeshFilter sourceMeshFilter = source.GetComponent<MeshFilter>();
        if (sourceMeshFilter != null)
        {
            MeshFilter destMeshFilter = destination.gameObject.AddComponent<MeshFilter>();
            destMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;
        }
        
        // Copy MeshRenderer
        MeshRenderer sourceMeshRenderer = source.GetComponent<MeshRenderer>();
        if (sourceMeshRenderer != null)
        {
            MeshRenderer destMeshRenderer = destination.gameObject.AddComponent<MeshRenderer>();
            destMeshRenderer.sharedMaterials = sourceMeshRenderer.sharedMaterials;
        }
        
        // Copy SkinnedMeshRenderer (for animated characters)
        SkinnedMeshRenderer sourceSkinnedMesh = source.GetComponent<SkinnedMeshRenderer>();
        if (sourceSkinnedMesh != null)
        {
            SkinnedMeshRenderer destSkinnedMesh = destination.gameObject.AddComponent<SkinnedMeshRenderer>();
            destSkinnedMesh.sharedMesh = sourceSkinnedMesh.sharedMesh;
            destSkinnedMesh.sharedMaterials = sourceSkinnedMesh.sharedMaterials;
            
            // Copy bone structure if needed
            if (sourceSkinnedMesh.bones != null && sourceSkinnedMesh.bones.Length > 0)
            {
                // Create bone hierarchy
                Transform[] newBones = new Transform[sourceSkinnedMesh.bones.Length];
                for (int i = 0; i < sourceSkinnedMesh.bones.Length; i++)
                {
                    if (sourceSkinnedMesh.bones[i] != null)
                    {
                        GameObject boneObj = new GameObject(sourceSkinnedMesh.bones[i].name);
                        boneObj.transform.SetParent(destination);
                        boneObj.transform.localPosition = sourceSkinnedMesh.bones[i].localPosition;
                        boneObj.transform.localRotation = sourceSkinnedMesh.bones[i].localRotation;
                        boneObj.transform.localScale = sourceSkinnedMesh.bones[i].localScale;
                        newBones[i] = boneObj.transform;
                    }
                }
                destSkinnedMesh.bones = newBones;
                destSkinnedMesh.rootBone = newBones.Length > 0 ? newBones[0] : null;
            }
        }
        
        // Copy SpriteRenderer (if using 2D sprites)
        SpriteRenderer sourceSpriteRenderer = source.GetComponent<SpriteRenderer>();
        if (sourceSpriteRenderer != null)
        {
            SpriteRenderer destSpriteRenderer = destination.gameObject.AddComponent<SpriteRenderer>();
            destSpriteRenderer.sprite = sourceSpriteRenderer.sprite;
            destSpriteRenderer.material = sourceSpriteRenderer.material;
            destSpriteRenderer.color = sourceSpriteRenderer.color;
            destSpriteRenderer.sortingLayerID = sourceSpriteRenderer.sortingLayerID;
            destSpriteRenderer.sortingOrder = sourceSpriteRenderer.sortingOrder;
        }
        
        // Copy Animator (optional - if you want animations to play)
        Animator sourceAnimator = source.GetComponent<Animator>();
        if (sourceAnimator != null)
        {
            Animator destAnimator = destination.gameObject.AddComponent<Animator>();
            destAnimator.runtimeAnimatorController = sourceAnimator.runtimeAnimatorController;
            destAnimator.avatar = sourceAnimator.avatar;
            destAnimator.applyRootMotion = false; // Disable root motion for display
        }
        
        // Recursively copy child objects
        foreach (Transform child in source)
        {
            GameObject childCopy = new GameObject(child.name);
            childCopy.transform.SetParent(destination);
            childCopy.transform.localPosition = child.localPosition;
            childCopy.transform.localRotation = child.localRotation;
            childCopy.transform.localScale = child.localScale;
            
            CopyVisualComponents(child, childCopy.transform);
        }
    }
    
    private void ClearPlayerModels()
    {
        foreach (var model in currentPlayerModels)
        {
            if (model != null)
                Destroy(model);
        }
        currentPlayerModels.Clear();
    }
}