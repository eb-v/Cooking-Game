using NUnit.Framework;
using UnityEditor.SearchService;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu Objects")]
    [SerializeField] private GameObject _loadingBarGameObject;
    [SerializeField] private GameObject[] _objectsToHide;

    [Header("Scenes to Load")]
    [SerializeField] private SceneField _playScene;
    [SerializeField] private SceneField _persistentGamePlay;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private void Awake()
    {
        _loadingBarGameObject.SetActive(false);
    }

    public void StartGame()
    {
        HideMenu();

        _loadingBarGameObject.SetActive(true);

        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_persistentGamePlay));
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_playScene, LoadSceneMode.Additive));

        StartCoroutine(ProgressLoadingBar());
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void HideMenu()
    {
        for (int i = 0; i < _objectsToHide.Length; i++)
        {
            _objectsToHide[i].SetActive(false);
        }
    }

    private IEnumerator ProgressLoadingBar()
    {
        float loadProgress = 0f;
        Slider loadingBarSlider = _loadingBarGameObject.GetComponent<Slider>();

        for (int i = 0; i < _scenesToLoad.Count; i++)
        {
            while (!_scenesToLoad[i].isDone)
            {
                loadProgress += _scenesToLoad[i].progress;
                loadingBarSlider.value = loadProgress / _scenesToLoad.Count;
                yield return null;
            }
        }
    }


}
