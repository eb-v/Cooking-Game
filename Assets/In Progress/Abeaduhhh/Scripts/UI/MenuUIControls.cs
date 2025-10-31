//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class MenuSpringController : MonoBehaviour {
//    [Header("Buttons")]
//    [SerializeField] private SpringAPI playSpring;
//    [SerializeField] private SpringAPI quitSpring;
//    [SerializeField] private SpringAPI titleSpring;

//    [Header("Animation Settings")]
//    [SerializeField] private float springInGoal = 1f;
//    [SerializeField] private float springOutGoal = -1f;

//    private bool hasStarted = false;

//    private void Start() {
//        playSpring.ResetSpring(); // starting position off screen...
//        quitSpring.ResetSpring();
//        titleSpring.ResetSpring();

//        Invoke(nameof(SpringInMenu), 0.2f);
//    }

//    private void SpringInMenu() {
//        playSpring.SetGoalValue(springInGoal);
//        quitSpring.SetGoalValue(springInGoal);
//        titleSpring.SetGoalValue(springInGoal);

//        playSpring.NudgeSpringVelocity();
//        quitSpring.NudgeSpringVelocity();
//        titleSpring.NudgeSpringVelocity();
//    }

//    public void OnPlayClicked() {
//        if (hasStarted) return;
//        hasStarted = true;

//        SpringOutAndStartGame();
//    }

//    public void OnQuitClicked() {
//        Application.Quit();
//    }

//    private void SpringOutAndStartGame() {
//        playSpring.SetGoalValue(springOutGoal);
//        quitSpring.SetGoalValue(springOutGoal);
//        titleSpring.SetGoalValue(springOutGoal);

//        playSpring.NudgeSpringVelocity();
//        quitSpring.NudgeSpringVelocity();
//        titleSpring.SetGoalValue(springOutGoal);

//        // Wait until animation finishes, then load game
//        Invoke(nameof(StartGame), 1f);
//    }

//    private void StartGame() {
//        SceneManager.LoadScene("GameScene");
//    }
//}
