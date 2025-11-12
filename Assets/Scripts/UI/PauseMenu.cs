using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    public GameObject settingsPanel;
    public GameObject pauseMenuContent; // The main pause menu buttons/content

    [Header("Settings UI Elements")]
    public Slider volumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown controllerTypeDropdown;
    public Image controllerImage;

    [Header("Controller Images")]
    public Sprite ps5ControllerSprite;
    public Sprite xboxControllerSprite;

    private PlayerInput playerInput;
    private bool isPausedByMenu = false;
    private Resolution[] resolutions;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions["Pause"].performed += OnPause;
        }

        // Initialize settings panel as hidden
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Setup resolution dropdown
        SetupResolutionDropdown();

        // Setup volume slider
        if (volumeSlider != null)
        {
            // Set default volume to 0.5 (50%)
            AudioListener.volume = 0.5f;
            volumeSlider.value = 50f;  // Slider goes 0-100, so 50 is middle
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Setup controller dropdown
        if (controllerTypeDropdown != null)
        {
            controllerTypeDropdown.onValueChanged.AddListener(OnControllerTypeChanged);
            // Set initial controller image
            OnControllerTypeChanged(controllerTypeDropdown.value);
        }
    }

    void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio.value.ToString("F0") + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
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

        // If settings panel is open, close it and return to pause menu
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }

        // Otherwise toggle the pause menu
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
        
        // Clear player stats
        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.ClearAllPlayers();
            Debug.Log("Player stats cleared");
        }
        
        // Clear and destroy all players
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ClearAllPlayers();
            Debug.Log("All players cleared and destroyed");
        }
        
        // Load main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        // Hide the pause menu content when settings opens
        if (pauseMenuContent != null)
        {
            pauseMenuContent.SetActive(false);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Show the pause menu content when settings closes
        if (pauseMenuContent != null)
        {
            pauseMenuContent.SetActive(true);
        }
    }

    void OnVolumeChanged(float value)
    {
        // Convert slider value (0-100) to audio volume (0-1)
        AudioListener.volume = value / 100f;
    }

    void OnResolutionChanged(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    void OnControllerTypeChanged(int index)
    {
        if (controllerImage == null) return;

        switch (index)
        {
            case 0: // PS5
                if (ps5ControllerSprite != null)
                {
                    controllerImage.sprite = ps5ControllerSprite;
                }
                break;
            case 1: // Xbox
                if (xboxControllerSprite != null)
                {
                    controllerImage.sprite = xboxControllerSprite;
                }
                break;
        }
    }
}