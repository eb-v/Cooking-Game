using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingButton;



    //listeners for the buttons on Awake
    private void Awake()
    {
        playButton.onClick.AddListener(PlayClick);
        quitButton.onClick.AddListener(QuitClick);
        settingButton.onClick.AddListener(SettingsClick);
    }

    private void PlayClick()
    {
        // SceneManager.LoadScene(1); loads the scene but with a lag


        Loader.Load(Loader.Scene.PreDuelSceneAudio1); //loads the loading screen first then PreDuelSceneAudio1
    }

    private void QuitClick()
    {
        Application.Quit(); //works on full build but not in editor
    }

    private void SettingsClick()
    {
         //does nothing for now
    }


}
