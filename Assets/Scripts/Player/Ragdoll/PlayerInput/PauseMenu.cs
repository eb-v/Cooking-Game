using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Pause"].performed += OnPause;
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
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        // Check if the container GameObject still exists
        if (container == null)
        {
            Debug.LogWarning("Pause menu container is missing!");
            return; // Exit the function to prevent the error
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
    }

    public void ResumeGame()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }

    public void MainMenuButton()
    {
    Time.timeScale = 1;
    UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu Scene");
    }
}