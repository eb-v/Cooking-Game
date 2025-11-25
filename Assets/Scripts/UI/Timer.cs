using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using IdyllicFantasyNature;

public class Timer : MonoBehaviour {
    [Header("Timer Settings")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTime = 300f;
    [SerializeField] private float sceneTransitionDelay = 1f;

    private float timeRemaining;
    private bool gameOver = false;
    private bool timerOn = true;
    private bool finalThreeSFXStarted = false;

    private void Start() {
        timeRemaining = startTime;
        finalThreeSFXStarted = false;
        DisplayTime(timeRemaining);
    }

    private void Update() {
        if (!timerOn || gameOver || Time.timeScale <= 0)
            return;

        if (timeRemaining > 0) {
            timeRemaining -= Time.deltaTime;
            float clamped = Mathf.Max(0f, timeRemaining);
            DisplayTime(clamped);
            GenericEvent<GameTimeUpdatedEvent>.GetEvent("Timer").Invoke(clamped);

            if (!finalThreeSFXStarted && clamped > 0f && clamped <= 3f) {
                finalThreeSFXStarted = true;
                PlayFinalThreeSecondsSFX();
            }
        } else {
            timeRemaining = 0;
            DisplayTime(0);
            gameOver = true;
            timerOn = false;
            GenericEvent<GameOverEvent>.GetEvent("Timer").Invoke();
            GenericEvent<ShowEndGameUIEvent>.GetEvent("EndGameUI").Invoke();
            Debug.Log("starting end game ui event");
            Debug.Log("[Timer] Game over event invoked.");
            
            // Load AwardsScene after delay
            StartCoroutine(LoadAwardsSceneCoroutine());
        }
    }

    private void DisplayTime(float timeToDisplay) {
        timeToDisplay = Mathf.Max(0, timeToDisplay);
        timeToDisplay = Mathf.Ceil(timeToDisplay);  // round up
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StartTimer() {
        timerOn = true;
        if (timeRemaining > 3f) finalThreeSFXStarted = false;
    }

    public void StopTimer() => timerOn = false;

    private void PlayFinalThreeSecondsSFX() {
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX("TimerEnd");
        } else {
            Debug.LogWarning("[Timer] Wanted to play TimerEnd SFX, but AudioManager.Instance is null.");
        }
    }

    private IEnumerator LoadAwardsSceneCoroutine() {
        // Wait for the delay
        yield return new WaitForSeconds(sceneTransitionDelay);
        
        // Find which level scene is currently loaded
        Scene levelScene = default;
        string levelSceneName = "";
        if (SceneManager.GetSceneByName("Level 1").isLoaded) {
            levelScene = SceneManager.GetSceneByName("Level 1");
            levelSceneName = "Level 1";
        } else if (SceneManager.GetSceneByName("Level 2").isLoaded) {
            levelScene = SceneManager.GetSceneByName("Level 2");
            levelSceneName = "Level 2";
        }
        
        if (levelScene.isLoaded) {
            Debug.Log($"[Timer] Found loaded level scene: {levelSceneName}");
            
            // Reset time scale before loading awards scene
            Time.timeScale = 1f;
            
            // Load AwardsScene additively
            var awardsLoad = SceneManager.LoadSceneAsync("AwardsScene", LoadSceneMode.Additive);
            while (!awardsLoad.isDone)
                yield return null;
            
            // Set AwardsScene as the active scene
            Scene awardsScene = SceneManager.GetSceneByName("AwardsScene");
            SceneManager.SetActiveScene(awardsScene);
            
            // Wait one frame to ensure scene is fully initialized
            yield return null;
            
            // Define spawn positions for each player
            Vector3[] spawnPositions = new Vector3[] {
                new Vector3(-15f, 10.75f, 45f),  // First player
                new Vector3(0f, 10.75f, 45f),    // Second player
                new Vector3(-20f, 10.75f, 45f),  // Third player
                new Vector3(5f, 10.75f, 45f)     // Fourth player
            };
            
            // Default rotation (facing forward)
            Quaternion defaultRotation = Quaternion.identity;
            
            // Setup players in awards scene
            var players = PlayerManager.Instance.Players;
            yield return StartCoroutine(SetupPlayersInAwardsScene(players, spawnPositions, defaultRotation));
            
            Debug.Log("[Timer] Moved players to spawn positions, reset rotations, froze physics, and disabled RagdollController in AwardsScene");
            
            // Unload the level scene
            var levelSceneUnload = SceneManager.UnloadSceneAsync(levelScene);
            while (!levelSceneUnload.isDone)
                yield return null;
            
            Debug.Log($"[Timer] Transitioned to AwardsScene and unloaded {levelSceneName}. Persistent scene remains loaded.");
        } else {
            Debug.LogError("[Timer] Could not find Level 1 or Level 2 scene loaded!");
        }
    }

private IEnumerator SetupPlayersInAwardsScene(System.Collections.Generic.List<GameObject> players, Vector3[] spawnPositions, Quaternion defaultRotation) {
    for (int i = 0; i < players.Count && i < spawnPositions.Length; i++) {
        if (players[i] != null) {
            // Get RagdollController to access the ROOT rigidbody
            RagdollController rc = null;
            if (players[i].TryGetComponent<RagdollController>(out rc)) {
                rc.enabled = false;
                Debug.Log($"[Timer] Disabled RagdollController for player {i}");
            }
            
            // Disable CharacterController if present
            CharacterController cc = null;
            if (players[i].TryGetComponent<CharacterController>(out cc)) {
                cc.enabled = false;
            }
            
            // Freeze all physics BEFORE setting position
            Rigidbody[] allRigidbodies = players[i].GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in allRigidbodies) {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            
            // Wait a frame for physics to settle
            yield return null;
            
            // Reset to initial position to "clean up" the ragdoll state
            if (rc != null && rc.InitialPosition != Vector3.zero) {
                // Reset the ROOT rigidbody position (the actual ragdoll body)
                if (rc.RagdollDict.ContainsKey(RagdollController.ROOT)) {
                    // Access the transform through the RagdollJoint
                    Transform rootTransform = rc.RagdollDict[RagdollController.ROOT].Rigidbody.transform;
                    rootTransform.position = rc.InitialPosition;
                    rootTransform.rotation = rc.InitialRotation;
                }
                
                // Also reset parent transform
                players[i].transform.position = rc.InitialPosition;
                players[i].transform.rotation = rc.InitialRotation;
                
                Debug.Log($"[Timer] Reset player {i} ROOT to initial position: {rc.InitialPosition}");
            }
            
            // Wait another frame for reset to take effect
            yield return null;
            
            // NOW move to the awards scene spawn position
            // Move both the parent AND the ROOT rigidbody
            if (rc != null && rc.RagdollDict.ContainsKey(RagdollController.ROOT)) {
                Transform rootTransform = rc.RagdollDict[RagdollController.ROOT].Rigidbody.transform;
                rootTransform.position = spawnPositions[i];
                rootTransform.rotation = defaultRotation;
            }
            
            players[i].transform.position = spawnPositions[i];
            players[i].transform.rotation = defaultRotation;
            
            // Wait another frame
            yield return null;
            
            // Re-enable CharacterController if it was disabled
            if (cc != null) {
                cc.enabled = true;
            }
            
            // Force rotation one more time to be absolutely sure
            players[i].transform.rotation = defaultRotation;
            if (rc != null && rc.RagdollDict.ContainsKey(RagdollController.ROOT)) {
                rc.RagdollDict[RagdollController.ROOT].Rigidbody.transform.rotation = defaultRotation;
            }
            
            Debug.Log($"[Timer] Moved player {i} ROOT to awards spawn position: {spawnPositions[i]}");
        }
    }
}
}