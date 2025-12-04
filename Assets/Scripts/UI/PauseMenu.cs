using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    public GameObject pauseMenuContent;
    public GameObject controllerImageObject;

    [Header("Settings UI Elements")]
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public AudioMixer audioMixer;

    [Header("Navigation")]
    public Button resumeButton;

    private PlayerInput playerInput;
    private bool isPausedByMenu = false;
    private EventSystem eventSystem;

    private Image sfxHandleImage;
    private Image musicHandleImage;
    private Color normalHandleColor = Color.white;
    private Color selectedHandleColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Darker gray


    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions["Pause"].performed += OnPause;
        }

        // Get EventSystem reference
        eventSystem = EventSystem.current;

        // Hide the entire pause menu on start
        if (container != null)
        {
            container.SetActive(false);
        }

        // Make sure pause menu content is visible, controller image is hidden
        if (pauseMenuContent != null)
        {
            pauseMenuContent.SetActive(true);
        }

        if (controllerImageObject != null)
        {
            controllerImageObject.SetActive(false);
        }

        // Initialize SFX slider
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = 75f;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            OnSFXVolumeChanged(sfxVolumeSlider.value);
        }

        // Initialize Music slider
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = 75f;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            OnMusicVolumeChanged(musicVolumeSlider.value);
        }

        // Get handle images for visual feedback
        if (sfxVolumeSlider != null)
        {
            sfxHandleImage = sfxVolumeSlider.handleRect?.GetComponent<Image>();
        }

        if (musicVolumeSlider != null)
        {
            musicHandleImage = musicVolumeSlider.handleRect?.GetComponent<Image>();
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        // Check for Select/Back button on gamepad
        if (Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame)
        {
            TogglePause();
        }

        // Update slider handle colors based on selection
        UpdateSliderHandleColors();
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
            FreezeManager.PauseMenuOverride = false;
            FreezeManager.ApplyState();

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

        // If controls are open, go back to pause menu
        if (controllerImageObject != null && controllerImageObject.activeSelf)
        {
            CloseControls();
            return;
        }

        // Toggle pause menu on/off
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

        FreezeManager.PauseMenuOverride = true;
        FreezeManager.ApplyState();

        isPausedByMenu = true;

        if (GameStartCountdownUI.CountdownIsActive)
        {
            GameStartCountdownUI.CountdownIsPaused = true;
        }

        // Set Resume button as first selected for keyboard/gamepad navigation
        if (eventSystem != null && resumeButton != null)
        {
            eventSystem.SetSelectedGameObject(resumeButton.gameObject);
        }

        // Pause SFX
        AudioManager.Instance?.PlaySFX("Pause");

        Debug.Log("Game paused by pause menu");
    }

    public void ResumeGame()
    {
        container.SetActive(false);

        FreezeManager.PauseMenuOverride = false;
        FreezeManager.ApplyState();

        if (GameStartCountdownUI.CountdownIsActive)
        {
            GameStartCountdownUI.CountdownIsPaused = false;
        }

        isPausedByMenu = false;

        // Clear selected object when closing menu
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }

        // Unpause SFX
        AudioManager.Instance?.PlaySFX("Unpause");

        Debug.Log("Game resumed from pause menu");
    }

    public void MainMenuButton()
    {
        FreezeManager.PauseMenuOverride = false;
        FreezeManager.ApplyState();

        isPausedByMenu = false;
        GameStartCountdownUI.CountdownIsPaused = false;

        // Clear selected object when leaving scene
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }

        PlayerStatsManager.ClearAllPlayers();
        PlayerManager.Instance.ClearAllPlayers();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }

    public void OpenControls()
    {
        if (pauseMenuContent != null)
            pauseMenuContent.SetActive(false);

        if (controllerImageObject != null)
            controllerImageObject.SetActive(true);
    }

    public void CloseControls()
    {
        if (controllerImageObject != null)
            controllerImageObject.SetActive(false);

        if (pauseMenuContent != null)
            pauseMenuContent.SetActive(true);
    }

    void OnSFXVolumeChanged(float value)
    {
        if (audioMixer != null)
        {
            // Convert 0-100 slider to decibels (-80 to 0)
            // When slider is at 0, volume should be -80dB (essentially muted)
            // When slider is at 100, volume should be 0dB (full volume)
            float volume = value > 0 ? Mathf.Log10(value / 100f) * 20f : -80f;
            bool success = audioMixer.SetFloat("SFXVolume", volume);
            Debug.Log($"SFX Volume Changed: Slider={value}, Decibels={volume:F2}, Success={success}");
        }
        else
        {
            Debug.LogWarning("AudioMixer is not assigned in PauseMenu!");
        }
    }

    void OnMusicVolumeChanged(float value)
    {
        if (audioMixer != null)
        {
            // Convert 0-100 slider to decibels (-80 to 0)
            float volume = value > 0 ? Mathf.Log10(value / 100f) * 20f : -80f;
            bool success = audioMixer.SetFloat("MusicVolume", volume);
            Debug.Log($"Music Volume Changed: Slider={value}, Decibels={volume:F2}, Success={success}");
        }
        else
        {
            Debug.LogWarning("AudioMixer is not assigned in PauseMenu!");
        }
    }

    void UpdateSliderHandleColors()
    {
        if (eventSystem == null) return;

        GameObject selected = eventSystem.currentSelectedGameObject;

        // Update SFX slider handle
        if (sfxHandleImage != null)
        {
            if (selected == sfxVolumeSlider?.gameObject)
            {
                sfxHandleImage.color = selectedHandleColor;
            }
            else
            {
                sfxHandleImage.color = normalHandleColor;
            }
        }

        // Update Music slider handle
        if (musicHandleImage != null)
        {
            if (selected == musicVolumeSlider?.gameObject)
            {
                musicHandleImage.color = selectedHandleColor;
            }
            else
            {
                musicHandleImage.color = normalHandleColor;
            }
        }
    }
}
