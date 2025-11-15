using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneRoot : MonoBehaviour
{
    [SerializeField] private SceneField _persistantScene;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        Scene persistantScene = SceneManager.GetSceneByName(_persistantScene.SceneName);
        if (!persistantScene.isLoaded)
        {
            var sceneLoadOperation = SceneManager.LoadSceneAsync(_persistantScene.SceneName, LoadSceneMode.Additive);

            while (!sceneLoadOperation.isDone)
            {
                yield return null;
            }
        }
    }
}
