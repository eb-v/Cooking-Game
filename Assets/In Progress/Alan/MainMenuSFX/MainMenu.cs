using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;



public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private float loadDelaySeconds = 0.12f; //slight delay for sfx to play
    private bool _isLoading; 


    

    //listeners for the buttons on Awake
    private void Awake()
    {
        playButton.onClick.AddListener(PlayClick);
        quitButton.onClick.AddListener(QuitClick);
        settingButton.onClick.AddListener(SettingsClick);
    }

     private void PlayClick()
    {
        if (_isLoading) return;
        StartCoroutine(LoadAfterDelay());
    }

    private IEnumerator LoadAfterDelay()
    {
        _isLoading = true;

        yield return new WaitForSecondsRealtime(loadDelaySeconds);

        Loader.Load(Loader.Scene.Level1Scene);
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
