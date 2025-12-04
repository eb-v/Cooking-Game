using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    public static bool isPaused = false;

    private void OnEnable()
    {
        GenericEvent<OnPauseGameInput>.GetEvent("PauseManager").AddListener(OnPauseInput);
    }

    private void OnDisable()
    {
        GenericEvent<OnPauseGameInput>.GetEvent("PauseManager").RemoveListener(OnPauseInput);
    }

    private void OnPauseInput()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
    }

    private void ResumeGame()
    {
        pauseMenu.SetActive(false);
    }
}
