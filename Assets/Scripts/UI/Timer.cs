using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float startTime = 300f; // 5 minutes in seconds
    [SerializeField] GameObject gameOverCanvas; // Assign your canvas with replay/menu buttons
    
    float timeRemaining;
    bool timerStarted = false;
    bool gameOver = false;
    
    void Start()
    {
        timeRemaining = startTime;
        
        // Make sure the game over canvas is hidden at start
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        
        // Subscribe to state changes
        if (KitchenGameManager.Instance != null)
        {
            KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        }
    }
    
    void OnDestroy()
    {
        if (KitchenGameManager.Instance != null)
        {
            KitchenGameManager.Instance.OnStateChanged -= KitchenGameManager_OnStateChanged;
        }
    }
    
    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        // Start the timer when the game is playing
        if (KitchenGameManager.Instance.IsGamePlaying())
        {
            timerStarted = true;
        }
    }
    
    void Update()
    {
        if (!timerStarted || gameOver) return;
        
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
        
        // Optional: Disable player movement through KitchenGameManager
        // KitchenGameManager.Instance.PauseGame(); // if you have this method
    }
    
    // Call this from your Replay button
    public void ReplayButton()
    {
        Time.timeScale = 1f; // Unfreeze time
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    // Call this from your Main Menu button
    public void MainMenuButton()
    {
        Time.timeScale = 1f; // Unfreeze time
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu Scene");
    }
}