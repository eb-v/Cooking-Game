using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    private PlayerInput playerInput;
    private bool isPausedByMenu = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions["Pause"].performed += OnPause;
        }
    }

    void Update()
    {
        // Check for Escape key press every frame
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Pause"].performed += OnPause;
        }
    }

    void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Pause"].performed -= OnPause;
        }
        
        // If disabled while paused by menu, unpause
        if (isPausedByMenu)
        {
            // Only unpause if countdown isn't active
            if (!GameStartCountdownUI.CountdownIsActive)
            {
                Time.timeScale = 1;
            }
            GameStartCountdownUI.CountdownIsPaused = false;
            isPausedByMenu = false;
        }
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    private void TogglePause()
    {
        // Check if the container GameObject still exists
        if (container == null)
        {
            Debug.LogWarning("Pause menu container is missing!");
            return;
        }

        if (container.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        container.SetActive(true);
        Time.timeScale = 0;
        isPausedByMenu = true;
        
        // If countdown is active, pause it too
        if (GameStartCountdownUI.CountdownIsActive)
        {
            GameStartCountdownUI.CountdownIsPaused = true;
            Debug.Log("Countdown paused by pause menu");
        }
        
        Debug.Log("Game paused by pause menu");
    }

    public void ResumeGame()
    {
        container.SetActive(false);
        
        // Resume countdown if it was paused
        if (GameStartCountdownUI.CountdownIsActive)
        {
            GameStartCountdownUI.CountdownIsPaused = false;
            Debug.Log("Countdown resumed from pause menu");
        }
        
        // Only unpause if countdown isn't active
        // (If countdown is active, it will handle unpausing when done)
        if (!GameStartCountdownUI.CountdownIsActive)
        {
            Time.timeScale = 1;
        }
        
        isPausedByMenu = false;
        Debug.Log("Game resumed from pause menu");
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1;
        isPausedByMenu = false;
        GameStartCountdownUI.CountdownIsPaused = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu Scene");
    }
}