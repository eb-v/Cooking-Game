using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButtonsController : MonoBehaviour {
    [SerializeField] private GameObject buttonsContainer;

    private void Awake() {
        if (buttonsContainer != null)
            buttonsContainer.SetActive(false);
    }

    public void ShowButtons() {
        if (buttonsContainer != null)
            buttonsContainer.SetActive(true);
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenuScene");
    }

    //public void RestartGame() {
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    // I think we can reset the player stats here

   
    //}

}
