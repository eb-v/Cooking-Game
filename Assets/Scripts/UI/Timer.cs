using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float startTime = 300f; // 5 minutes in seconds
    [SerializeField] GameObject gameOverCanvas; // Assign your canvas with replay/menu buttons

    float timeRemaining;
    bool gameOver = false;
    bool hasStarted = false;
    float startDelay = 0f;

    void Start()
    {
        timeRemaining = startTime;
        // Make sure the game over canvas is hidden at start
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        
        // Display initial time
        DisplayTime(timeRemaining);
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
        // Freeze the game
        Time.timeScale = 0f;
        // Show the game over canvas with replay/menu options
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
    }

    // Call this from your Replay button
    public void ReplayButton()
    {
        Time.timeScale = 1f;
        
        // Reset points before reloading scene
        if (PointManager.Instance != null)
        {
            PointManager.Instance.ResetPoints();
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // Call this from your Main Menu button
    public void MainMenuButton()
    {
        Time.timeScale = 1f; // Unfreeze time
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu Scene");
    }
}