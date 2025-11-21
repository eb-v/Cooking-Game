using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu Objects")]
    [SerializeField] private GameObject _loadingBarGameObject;
    [SerializeField] private GameObject[] _objectsToHide;

    [Header("Scenes to Load")]
    [SerializeField] private SceneField _playScene;
    [SerializeField] private SceneField _persistentGamePlay;

    // --- Visual Tweaks (minimal, safe to remove) ---
    [Header("Visual Tweaks (Loading Bar Colors)")]
    [SerializeField] private Color _loadingBarBackground = new Color(0.10f, 0.10f, 0.14f, 1f); // dark indigo
    [SerializeField] private Color _loadingBarFill = new Color(0.27f, 0.76f, 1f, 1f);          // neon-cyan

    private readonly List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private void Awake()
    {
        if (_loadingBarGameObject) _loadingBarGameObject.SetActive(false);
        ApplyLoadingBarColors(); // <- purely visual
        // SceneManager.LoadSceneAsync(_persistentGamePlay, LoadSceneMode.Additive);
    }

    public void StartGame()
    {
        HideMenu();

        if (_loadingBarGameObject) _loadingBarGameObject.SetActive(true);

        // (kept as-is) If you intended additive for both, change the first line to Additive later
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_playScene, LoadSceneMode.Additive));
        SceneManager.UnloadSceneAsync("MainMenuScene");

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
        if (_objectsToHide == null) return;
        for (int i = 0; i < _objectsToHide.Length; i++)
            if (_objectsToHide[i]) _objectsToHide[i].SetActive(false);
    }

    private IEnumerator ProgressLoadingBar()
    {
        float loadProgress = 0f;
        Slider loadingBarSlider = _loadingBarGameObject ? _loadingBarGameObject.GetComponent<Slider>() : null;

        for (int i = 0; i < _scenesToLoad.Count; i++)
        {
            while (!_scenesToLoad[i].isDone)
            {
                loadProgress += _scenesToLoad[i].progress;
                if (loadingBarSlider)
                    loadingBarSlider.value = loadProgress / _scenesToLoad.Count;
                yield return null;
            }
        }

        // set play scene as active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_playScene.SceneName));
        GameManager.Instance.ChangeState(GameManager.Instance._lobbyStateInstance);
    }

    // === Minimal visual helper ===
    private void ApplyLoadingBarColors()
    {
        if (!_loadingBarGameObject) return;

        // Typical Slider hierarchy:
        // Slider
        //  ├─ Background (Image)
        //  └─ Fill Area
        //       └─ Fill (Image)   <-- this is what we want to tint
        var slider = _loadingBarGameObject.GetComponent<Slider>();
        if (!slider) return;

        // Tint Background (if present)
        var bg = _loadingBarGameObject.transform.Find("Background");
        if (bg)
        {
            var bgImg = bg.GetComponent<Image>();
            if (bgImg) bgImg.color = _loadingBarBackground;
        }

        // Tint Fill (if present)
        var fill = _loadingBarGameObject.transform.Find("Fill Area/Fill");
        if (fill)
        {
            var fillImg = fill.GetComponent<Image>();
            if (fillImg) fillImg.color = _loadingBarFill;
        }
        else
        {
            // Fallback: try to find any child Image named "Fill"
            var imgs = _loadingBarGameObject.GetComponentsInChildren<Image>(true);
            foreach (var img in imgs)
            {
                if (img.name.ToLower().Contains("fill"))
                {
                    img.color = _loadingBarFill;
                    break;
                }
            }
        }
    }
}