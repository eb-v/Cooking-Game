using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{ //names must be exact
    public enum Scene
    {
        MainMenu,
        PreDuelSceneAudio1,
        Loading
    }
    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.Loading.ToString());
    }
    
    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
