using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOverButtonsController : MonoBehaviour 
{
    [SerializeField] private GameObject buttonsContainer;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Color selectedColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Grey shade
    
    private GameObject _lastSelectedButton;

    private void Awake() 
    {
        if (buttonsContainer != null)
            buttonsContainer.SetActive(false);

        // Set up button colors
        if (mainMenuButton != null)
        {
            ColorBlock colors = mainMenuButton.colors;
            colors.highlightedColor = selectedColor;
            colors.selectedColor = selectedColor;
            mainMenuButton.colors = colors;
        }
    }

    private void Update()
    {
        // Check if selected button changed (via navigation)
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        
        if (currentSelected != _lastSelectedButton && currentSelected != null)
        {
            _lastSelectedButton = currentSelected;
        }

        // Fallback: if no button is selected, reselect the main menu button
        if (currentSelected == null && mainMenuButton != null && buttonsContainer.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(mainMenuButton.gameObject);
        }
    }

    public void ShowButtons() 
    {
        if (buttonsContainer != null)
        {
            buttonsContainer.SetActive(true);
            
            // Select the main menu button for controller navigation
            if (mainMenuButton != null)
            {
                EventSystem.current.SetSelectedGameObject(mainMenuButton.gameObject);
                _lastSelectedButton = mainMenuButton.gameObject;
            }
        }
    }

    public async void GoToMainMenu() 
    {
        // Unload Awards Scene if loaded
        if (SceneManager.GetSceneByName("Awards Scene").isLoaded)
        {
            await SceneManager.UnloadSceneAsync("Awards Scene");
        }
        
        // Load Main Menu Scene
        SceneManager.LoadScene("MainMenu");
    }

    //public void RestartGame() 
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //    // I think we can reset the player stats here
    //}
}