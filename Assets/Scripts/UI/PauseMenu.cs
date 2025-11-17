using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    public GameObject settingsPanel;
    public GameObject pauseMenuContent;

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

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        SetupResolutionDropdown();

        if (volumeSlider != null)
        {
            AudioListener.volume = 0.5f;
            volumeSlider.value = 50f;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (controllerTypeDropdown != null)
        {
            controllerTypeDropdown.onValueChanged.AddListener(OnControllerTypeChanged);
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
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + 
                            resolutions[i].refreshRateRatio.value.ToString("F0") + "Hz";
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

        if (isPausedByMenu)
        {
            FreezeManager.PauseMenuOverride = false;       // ⭐ NEW
            FreezeManager.ApplyState();                    // ⭐ NEW

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
        if (container == null)
        {
            Debug.LogWarning("Pause menu container is missing!");
            return;
        }

        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
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

        FreezeManager.PauseMenuOverride = true;   // ⭐ NEW
        FreezeManager.ApplyState();                // ⭐ NEW

        isPausedByMenu = true;

        if (GameStartCountdownUI.CountdownIsActive)
        {
            GameStartCountdownUI.CountdownIsPaused = true;
        }

        Debug.Log("Game paused by pause menu");
    }

    public void ResumeGame()
    {
        container.SetActive(false);

        FreezeManager.PauseMenuOverride = false;   // ⭐ NEW
        FreezeManager.ApplyState();                 // ⭐ NEW

        if (GameStartCountdownUI.CountdownIsActive)
        {
            GameStartCountdownUI.CountdownIsPaused = false;
        }

        isPausedByMenu = false;

        Debug.Log("Game resumed from pause menu");
    }

    public void MainMenuButton()
    {
        FreezeManager.PauseMenuOverride = false;   // ⭐ NEW
        FreezeManager.ApplyState();                 // ⭐ NEW

        isPausedByMenu = false;
        GameStartCountdownUI.CountdownIsPaused = false;

        PlayerStatsManager.ClearAllPlayers();
        PlayerManager.Instance.ClearAllPlayers();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (pauseMenuContent != null)
            pauseMenuContent.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (pauseMenuContent != null)
            pauseMenuContent.SetActive(true);
    }

    void OnVolumeChanged(float value)
    {
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
            case 0:
                if (ps5ControllerSprite != null)
                    controllerImage.sprite = ps5ControllerSprite;
                break;
            case 1:
                if (xboxControllerSprite != null)
                    controllerImage.sprite = xboxControllerSprite;
                break;
        }
    }
}
