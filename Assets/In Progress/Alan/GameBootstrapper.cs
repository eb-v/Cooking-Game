using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private SceneField persistentScene;
    [SerializeField] private SceneField mainMenuScene;

    private IEnumerator Start()
    {
        var persistentLoad = SceneManager.LoadSceneAsync(persistentScene, LoadSceneMode.Additive);
        while (!persistentLoad.isDone)
            yield return null;

        var menuLoad = SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);
        while (!menuLoad.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainMenuScene));

        SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
