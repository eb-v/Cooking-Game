using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private string persistentSceneName = "PersistentGamePlayScene";
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    private IEnumerator Start()
    {
        var persistentLoad = SceneManager.LoadSceneAsync(persistentSceneName, LoadSceneMode.Additive);
        while (!persistentLoad.isDone)
            yield return null;

        var menuLoad = SceneManager.LoadSceneAsync(mainMenuSceneName, LoadSceneMode.Additive);
        while (!menuLoad.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainMenuSceneName));

        SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
