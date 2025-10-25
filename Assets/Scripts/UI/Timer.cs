using UnityEngine;
using TMPro;
using System.Collections;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float startTime = 300f; // 5 minutes in seconds
    [SerializeField] GameObject gameOverCanvas; // Assign your canvas with replay/menu buttons
    [SerializeField] GameObject endGameCanvas; // Assign your EndGame statistics canvas
    [SerializeField] EndGameAwards endGameAwards;
    [SerializeField] private float awardDelay = 2f;
    
    float timeRemaining;
    bool gameOver = false;
    bool hasStarted = false;
    float startDelay = 0f;
    bool movementEnabled = false; // Track if we've enabled movement
    
    void Start()
    {
        timeRemaining = startTime;
        
        // Make sure the game over canvas is hidden at start
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        
        // Make sure the end game canvas is hidden at start
        if (endGameCanvas != null)
        {
            endGameCanvas.SetActive(false);
        }
        
        // Display initial time
        DisplayTime(timeRemaining);
        
        // Enable all player movement immediately at start
        EnableAllPlayerMovement();
    }
    
    private void EnableAllPlayerMovement()
    {
        if (PlayerManager.Instance != null)
        {
            foreach (var player in PlayerManager.Instance.GetAllPlayers())
            {
                RagdollController ragdoll = player.GetComponent<RagdollController>();
                if (ragdoll != null)
                {
                    ragdoll.TurnMovementOn();
                    Debug.Log("Turned on movement for Player " + player.playerNumber);
                }
            }
            movementEnabled = true;
        }
    }
    
    void Update()
    {
        if (gameOver) return;
        
        // Start counting when timeScale becomes 1 (game unpauses)
        if (Time.timeScale > 0)
        {
            if (!hasStarted)
            {
                hasStarted = true;
                startDelay = 0f;
            }

            // Add a small delay before first tick
            if (startDelay < 1f)
            {
                startDelay += Time.deltaTime;
                // Keep displaying the initial time during delay
                DisplayTime(timeRemaining);
                return;
            }
            
            // Now start counting down
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                // Time's up!
                timeRemaining = 0;
                timerText.text = "00:00";
                gameOver = true;
                GameOver();
            }
        }
    }
    
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay = Mathf.Max(0, timeToDisplay);
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void GameOver()
    {
        Debug.Log("Game Over - Starting Awards and Statistics");

        // Freeze the game
        Time.timeScale = 0f;

        // Turn on game over canvas
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
        
        // Turn on end game statistics canvas
        if (endGameCanvas != null)
        {
            endGameCanvas.SetActive(true);
        }
        
        // Show awards
        if (endGameAwards != null)
        {
            endGameAwards.ShowAwards();
        }
    }
    
    // Call this from your Replay button
    public void ReplayButton()
    {
        Debug.Log("Replay button pressed - Resetting game");
        
        Time.timeScale = 1f; // Unfreeze time
        
        // Reset points before reloading scene
        if (PointManager.Instance != null)
        {
            PointManager.Instance.ResetPoints();
        }
        
        // Reset all player statistics
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ResetAllStats();
        }

        
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    // Call this from your Main Menu button
    public void MainMenuButton()
    {
        Debug.Log("Main Menu button pressed");
        
        Time.timeScale = 1f; // Unfreeze time
        
        // Reset points
        if (PointManager.Instance != null)
        {
            PointManager.Instance.ResetPoints();
        }
        
        // Reset all player statistics
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ResetAllStats();
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu Scene");
    }
}