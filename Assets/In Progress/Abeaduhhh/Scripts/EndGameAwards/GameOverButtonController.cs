using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOverButtonsController : MonoBehaviour 
{
    [SerializeField] private GameObject buttonsContainer;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Color selectedColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Grey shade
    
    private ColorBlock originalColors;

    private void Awake() 
    {
        if (buttonsContainer != null)
            buttonsContainer.SetActive(false);

        if (mainMenuButton != null)
        {
            // Store original button colors
            originalColors = mainMenuButton.colors;
            
            // Set the selected/highlighted color to grey
            ColorBlock colors = mainMenuButton.colors;
            colors.highlightedColor = selectedColor;
            colors.selectedColor = selectedColor;
            mainMenuButton.colors = colors;
        }
    }

    public void ShowButtons() 
    {
        if (buttonsContainer != null)
            buttonsContainer.SetActive(true);
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