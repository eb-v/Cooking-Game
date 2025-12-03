using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    public GameObject pauseMenuContent;
    public GameObject controllerImageObject;

    [Header("Settings UI Elements")]
    public Slider volumeSlider;

    private PlayerInput playerInput;
    private bool isPausedByMenu = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions["Pause"].performed += OnPause;
        }

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

        if (volumeSlider != null)
        {
            AudioListener.volume = 0.5f;
            volumeSlider.value = 50f;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
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

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value / 100f;
    }
}
