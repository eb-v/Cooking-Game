// using UnityEngine;

// public class PauseMenuController : MonoBehaviour
// {
//     [SerializeField] private GameObject pauseMenuRoot;
//     private bool isPaused = false;

//     private void Start()
//     {
//         if (pauseMenuRoot != null)
//             pauseMenuRoot.SetActive(false);
//     }

//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Escape))
//         {
//             TogglePause();
//         }
//     }

//     public void TogglePause()
//     {
//         isPaused = !isPaused;

//         if (pauseMenuRoot != null)
//             pauseMenuRoot.SetActive(isPaused);

//         Time.timeScale = isPaused ? 0f : 1f;

//         if (UISFXManager.Instance != null)
//         {
//             if (isPaused)
//                 UISFXManager.Instance.PlayPauseOn();
//             else
//                 UISFXManager.Instance.PlayPauseOff();
//         }
//     }

//     public void ResumeFromButton()
//     {
//         if (!isPaused) return;

//         isPaused = false;

//         if (pauseMenuRoot != null)
//             pauseMenuRoot.SetActive(false);

//         Time.timeScale = 1f;

//         if (UISFXManager.Instance != null)
//             UISFXManager.Instance.PlayPauseOff();
//     }
// }
