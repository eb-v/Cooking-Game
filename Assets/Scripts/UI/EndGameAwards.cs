using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndGameAwards : MonoBehaviour
{
    [Header("Award Display")]
    [SerializeField] private GameObject awardDisplayPanel; // This IS your end game canvas
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
                PlayerManager.Instance.GetHotHandsWinners(),
                (player) => $"Ingredients: {player.ingredientsHandled}"
            ));
            
            if (ceremonyCancelled) yield break;
            
            // MVP Award
            yield return StartCoroutine(ShowAward(
                "MVP",
                "Most Points Generated",
                PlayerManager.Instance.GetMVPWinners(),
                (player) => $"Points: {player.pointsGenerated}"
            ));
            
            if (ceremonyCancelled) yield break;
            
            // Guardian Angel Award (only if someone revived)
            var guardianWinners = PlayerManager.Instance.GetGuardianAngelWinners();
            if (guardianWinners.Count > 0)
            {
                yield return StartCoroutine(ShowAward(
                    "Guardian Angel",
                    "Most Teammates Revived",
                    guardianWinners,
                    (player) => $"Revives: {player.teammatesRevived}"
                ));
            }
            
            if (ceremonyCancelled) yield break;
            
            // Noob Award
            yield return StartCoroutine(ShowAward(
                "Noob",
                "Least Points Generated",
                PlayerManager.Instance.GetNoobWinners(),
                (player) => $"Points: {player.pointsGenerated}"
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
        
        // Set award text
        if (awardTitleText != null)
            awardTitleText.text = title;
        
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
        
        if (playerNamesText != null)
            playerNamesText.text = playerNames;
        
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