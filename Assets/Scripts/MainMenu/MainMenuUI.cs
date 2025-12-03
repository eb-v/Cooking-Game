using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Field References")]
    [SerializeField] private SceneField _mainMenu;
    [SerializeField] private SceneField _lobby;
    [SerializeField] private SceneField _persistentGamePlay;

    [Header("Controller Navigation")]
    [SerializeField] private Button _firstSelectedButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    [Header("Button Highlight Color")]
    [SerializeField] private Color _highlightedColor = new Color(1f, 0.698f, 0f, 1f); // #FFB200

    // --- Visual Tweaks (minimal, safe to remove) ---
    [Header("Visual Tweaks (Loading Bar Colors)")]
    [SerializeField] private Color _loadingBarBackground = new Color(0.10f, 0.10f, 0.14f, 1f);
    [SerializeField] private Color _loadingBarFill = new Color(0.27f, 0.76f, 1f, 1f);

    private readonly List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();
    private GameObject _lastSelectedButton;

    private void Awake()
    {
        // Set up button colors and navigation
        SetupButtonColors();
        SetupButtonNavigation();
    }

    private void Start()
    {
        // Select the first button for controller navigation
        if (_firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(_firstSelectedButton.gameObject);
            _lastSelectedButton = _firstSelectedButton.gameObject;
        }
    }

    private void Update()
    {
        // Check if selected button changed (via navigation)
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        
        if (currentSelected != _lastSelectedButton && currentSelected != null)
        {
            _lastSelectedButton = currentSelected;
        }

        // Fallback: if no button is selected, reselect the first button
        if (currentSelected == null && _firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(_firstSelectedButton.gameObject);
        }
    }

    // NEW: Set button highlight colors
    private void SetupButtonColors()
    {
        SetButtonHighlightColor(_playButton, _highlightedColor);
        SetButtonHighlightColor(_quitButton, _highlightedColor);
    }

    private void SetButtonHighlightColor(Button button, Color highlightColor)
    {
        if (button == null) return;

        ColorBlock colors = button.colors;
        colors.highlightedColor = highlightColor;
        colors.selectedColor = highlightColor; // Also set selected color for controller
        button.colors = colors;
    }

    private void SetupButtonNavigation()
    {
        if (_playButton != null && _quitButton != null)
        {
            // Set Play button navigation
            Navigation playNav = _playButton.navigation;
            playNav.mode = Navigation.Mode.Explicit;
            playNav.selectOnDown = _quitButton;
            playNav.selectOnUp = _quitButton;
            _playButton.navigation = playNav;

            // Set Quit button navigation
            Navigation quitNav = _quitButton.navigation;
            quitNav.mode = Navigation.Mode.Explicit;
            quitNav.selectOnUp = _playButton;
            quitNav.selectOnDown = _playButton;
            _quitButton.navigation = quitNav;
        }
    }

    public void StartGame()
    {
        GameManager.Instance.SwitchScene(_mainMenu, _lobby, GameManager.Instance._lobbyStateInstance);
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
        //if (_objectsToHide == null) return;
        //for (int i = 0; i < _objectsToHide.Length; i++)
        //    if (_objectsToHide[i]) _objectsToHide[i].SetActive(false);
    }

    //private IEnumerator ProgressLoadingBar()
    //{
    //    //float loadProgress = 0f;
    //    //Slider loadingBarSlider = _loadingBarGameObject ? _loadingBarGameObject.GetComponent<Slider>() : null;
    //    //for (int i = 0; i < _scenesToLoad.Count; i++)
    //    //{
    //    //    while (!_scenesToLoad[i].isDone)
    //    //    {
    //    //        loadProgress += _scenesToLoad[i].progress;
    //    //        if (loadingBarSlider)
    //    //            loadingBarSlider.value = loadProgress / _scenesToLoad.Count;
    //    //        yield return null;
    //    //    }
    //    //}
    //    //// set play scene as active
    //    //SceneManager.SetActiveScene(SceneManager.GetSceneByName(_playScene.SceneName));
    //    //GameManager.Instance.ChangeState(GameManager.Instance._lobbyStateInstance);
    //}

    // === Minimal visual helper ===
    private void ApplyLoadingBarColors()
    {
        //if (!_loadingBarGameObject) return;
        //// Typical Slider hierarchy:
        //// Slider
        ////  ├─ Background (Image)
        ////  └─ Fill Area
        ////       └─ Fill (Image)   <-- this is what we want to tint
        //var slider = _loadingBarGameObject.GetComponent<Slider>();
        //if (!slider) return;
        //// Tint Background (if present)
        //var bg = _loadingBarGameObject.transform.Find("Background");
        //if (bg)
        //{
        //    var bgImg = bg.GetComponent<Image>();
        //    if (bgImg) bgImg.color = _loadingBarBackground;
        //}
        //// Tint Fill (if present)
        //var fill = _loadingBarGameObject.transform.Find("Fill Area/Fill");
        //if (fill)
        //{
        //    var fillImg = fill.GetComponent<Image>();
        //    if (fillImg) fillImg.color = _loadingBarFill;
        //}
        //else
        //{
        //    // Fallback: try to find any child Image named "Fill"
        //    var imgs = _loadingBarGameObject.GetComponentsInChildren<Image>(true);
        //    foreach (var img in imgs)
        //    {
        //        if (img.name.ToLower().Contains("fill"))
        //        {
        //            img.color = _loadingBarFill;
        //            break;
        //        }
        //    }
        //}
    }
}