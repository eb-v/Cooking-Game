using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum LevelModifiers {
    None = 0,
    Earthquake = 1,
    Lightning = 2,
    LandMines = 3,
    OilSpill = 4,
    CloseProximity = 5
}

public class SlotMachineScript : MonoBehaviour
{
    private const int MaxVisibleModifierIndex = 5;
    private const int MaxRandomExclusive = MaxVisibleModifierIndex + 1;

    [Header("Debug")]
    [SerializeField] private bool debugForceModifiers = false;
    [SerializeField] private LevelModifiers[] debugModifiers = new LevelModifiers[3];

    [Header("Slot SFX Names (AudioManager)")]
    [SerializeField] private string leverSfxName  = "Slot_Lever";
    [SerializeField] private string spinSfxName   = "Slot_Spin";
    [SerializeField] private string thumpSfxName  = "Slot_Thump";
    [SerializeField] private string winSfxName    = "Slot_Win";

    [Header("Cameras")]
    [SerializeField] private Camera slotMachineCamera;     // camera in this slot machine scene (NOT MainCamera)
    [SerializeField] private float resultHoldTime = 1.5f;  // how long to show final result

    private Camera gameplayCamera;                         // persistent gameplay camera
    private bool slotSequenceActive = false;

    [Header("Loading Screen")]
    [SerializeField] private Canvas loadingCanvas;
    [SerializeField] private float loadingFadeDuration = 0.5f; // fade in/out

    private CanvasGroup loadingCanvasGroup;

    private class SlotStruct {
        public NonNormalizedSpringAPI springSlot;
        public bool shouldSpin;
        public float spinDuration;
        public int finalGoal;
        public bool isDone;
    }

    [Header("Springs")]
    [SerializeField] private NonNormalizedSpringAPI springSlot1;
    [SerializeField] private NonNormalizedSpringAPI springSlot2;
    [SerializeField] private NonNormalizedSpringAPI springSlot3;

    [Header("Spin Durations")]
    [SerializeField] private float spinDuration1 = 2f;
    [SerializeField] private float spinDuration2 = 3f;
    [SerializeField] private float spinDuration3 = 4f;

    [SerializeField] private bool usePresetGoals = false;

    [Header("Lever Animation")]
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private string leverAnimStateName = "LeverPull";

    private SlotStruct slot1;
    private SlotStruct slot2;
    private SlotStruct slot3;

    [ReadOnly]
    [SerializeField] private List<LevelModifiers> _activeModifiers = new List<LevelModifiers>();

    private void Start()
    {
        if (loadingCanvas != null)
        {
            loadingCanvasGroup = loadingCanvas.GetComponent<CanvasGroup>();
            if (loadingCanvasGroup == null)
                loadingCanvasGroup = loadingCanvas.gameObject.AddComponent<CanvasGroup>();

            loadingCanvasGroup.alpha = 0f;
            loadingCanvas.gameObject.SetActive(false);
        }

        StartSlotMachine();
    }

    private void Update()
    {
        if (FreezeManager.PauseMenuOverride)
            return;

        if (slotSequenceActive)
        {
            // just make sure slot camera stays on while we're in the sequence
            if (slotMachineCamera != null && !slotMachineCamera.enabled)
                slotMachineCamera.enabled = true;
        }

        RunSpinCheckLogic(slot1);
        RunSpinCheckLogic(slot2);
        RunSpinCheckLogic(slot3);
    }

    public void StartSlotMachine()
    {
        // 🔹 Prevent double-starting slot sequence & audio
        if (slotSequenceActive)
            return;

        StartCoroutine(StartSlotMachineRoutine());
    }

    private IEnumerator StartSlotMachineRoutine()
    {
        slotSequenceActive = true;

        // freeze gameplay
        FreezeManager.FreezeGameplay();

        // cache cameras and switch to slot camera
        SetupCamerasForSlot();

        // tiny delay just to avoid any frame-order weirdness
        yield return null;

        // lever animation
        if (leverAnimator != null && !string.IsNullOrEmpty(leverAnimStateName))
        {
            leverAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            leverAnimator.Play(leverAnimStateName, 0, 0f);
        }

        LevelModifiers mod1, mod2, mod3;

        if (debugForceModifiers && debugModifiers != null && debugModifiers.Length >= 3)
        {
            mod1 = debugModifiers[0];
            mod2 = debugModifiers[1];
            mod3 = debugModifiers[2];
        }
        else
        {
            int num1, num2, num3;
            num1 = Random.Range(1, 6);
            do { num2 = Random.Range(1, 6); } while (num2 == num1);
            do { num3 = Random.Range(1, 6); } while (num3 == num1 || num3 == num2);

            mod1 = (LevelModifiers)num1;
            mod2 = (LevelModifiers)num2;
            mod3 = (LevelModifiers)num3;
        }

        // Start audio sequence (only once)
        StartCoroutine(SlotAudioRoutine());

        _activeModifiers.Clear();
        _activeModifiers.Add(mod1);
        _activeModifiers.Add(mod2);
        _activeModifiers.Add(mod3);

        Debug.Log($"[SlotMachine] Chosen Modifiers: {mod1}, {mod2}, {mod3}");

        slot1 = new SlotStruct {
            springSlot = springSlot1,
            shouldSpin = false,
            spinDuration = spinDuration1,
            finalGoal = (int)mod1
        };
        slot2 = new SlotStruct {
            springSlot = springSlot2,
            shouldSpin = false,
            spinDuration = spinDuration2,
            finalGoal = (int)mod2
        };
        slot3 = new SlotStruct {
            springSlot = springSlot3,
            shouldSpin = false,
            spinDuration = spinDuration3,
            finalGoal = (int)mod3
        };

        StartSpinningAll();
    }

    private void SetupCamerasForSlot()
    {
        if (slotMachineCamera == null)
            slotMachineCamera = GetComponentInChildren<Camera>();

        if (slotMachineCamera == null)
        {
            Debug.LogError("[SlotMachine] No slotMachineCamera assigned or found!");
            return;
        }

        if (gameplayCamera == null)
        {
            Camera main = Camera.main;
            if (main != null && main != slotMachineCamera)
            {
                gameplayCamera = main;
            }
        }

        // disable gameplay camera while we show the slot
        if (gameplayCamera != null)
            gameplayCamera.enabled = false;

        // enable slot camera
        slotMachineCamera.enabled = true;
    }

    private void RestoreGameplayCamera()
    {
        slotSequenceActive = false;

        // turn off slot cam
        if (slotMachineCamera != null)
            slotMachineCamera.enabled = false;

        // if somehow gameplayCamera wasn't cached, try one more time
        if (gameplayCamera == null)
        {
            Camera main = Camera.main;
            if (main != null && main != slotMachineCamera)
                gameplayCamera = main;
        }

        // turn gameplay cam back on
        if (gameplayCamera != null)
            gameplayCamera.enabled = true;
        else
            Debug.LogWarning("[SlotMachine] No gameplayCamera found to restore!");
    }

    public void RunUpdateLogic()
    {
        if (slot1 != null)
            RunSpinCheckLogic(slot1);
        if (slot2 != null)
            RunSpinCheckLogic(slot2);
        if (slot3 != null)
            RunSpinCheckLogic(slot3);
    }

    private void StartSlotSpin(SlotStruct slot)
    {
        NonNormalizedSpringAPI springAPI = slot.springSlot;
        float spinGoalValue = MaxRandomExclusive; // spin past last visible index
        springAPI.SetGoalValue(spinGoalValue);
        slot.shouldSpin = true;
    }

    private void StopSlotSpin(SlotStruct slot)
    {
        slot.shouldSpin = false;

        NonNormalizedSpringAPI springAPI = slot.springSlot;
        float newGoal = slot.finalGoal;

        Debug.Log($"[SlotMachine] Slot stopping at index {newGoal} ({(LevelModifiers)newGoal})");
        springAPI.SetGoalValue(newGoal);
        slot.isDone = true;

        CheckIfAllSlotsDone();
    }

    private void CheckIfAllSlotsDone()
    {
        if (slot1 != null && slot2 != null && slot3 != null &&
            slot1.isDone && slot2.isDone && slot3.isDone)
        {
            StartCoroutine(AllSlotsFinishedRoutine());
        }
    }

    private IEnumerator AllSlotsFinishedRoutine()
    {
        // show the final slot result
        yield return new WaitForSecondsRealtime(resultHoldTime);

        // handle loading screen/gameplay return
        yield return StartCoroutine(ShowLoadingThenReturnToGameplay());
    }

    private IEnumerator ShowLoadingThenReturnToGameplay()
    {
        if (loadingCanvas != null)
        {
            loadingCanvas.gameObject.SetActive(true);

            if (loadingCanvasGroup == null)
            {
                loadingCanvasGroup = loadingCanvas.GetComponent<CanvasGroup>();
                if (loadingCanvasGroup == null)
                    loadingCanvasGroup = loadingCanvas.gameObject.AddComponent<CanvasGroup>();
            }

            loadingCanvasGroup.alpha = 0f;
            yield return StartCoroutine(FadeCanvasGroup(loadingCanvasGroup, 0f, 1f, loadingFadeDuration));
        }

        // switch back to gameplay camera behind the loading screen
        RestoreGameplayCamera();

        // 🔹 Now wait until the player actually presses something (with debounce)
        yield return StartCoroutine(WaitForLoadingDismissInput());

        if (loadingCanvas != null && loadingCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(loadingCanvasGroup, 1f, 0f, loadingFadeDuration));
            loadingCanvas.gameObject.SetActive(false);
        }

        // unfreeze gameplay
        FreezeManager.UnfreezeGameplay();

        // fire modifiers event
        GenericEvent<OnModifiersChoosenEvent>
            .GetEvent("OnModifiersChoosenEvent")
            .Invoke(_activeModifiers);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            cg.alpha = to;
            yield break;
        }

        float t = 0f;
        cg.alpha = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            cg.alpha = Mathf.Lerp(from, to, normalized);
            yield return null;
        }

        cg.alpha = to;
    }

        private IEnumerator WaitForLoadingDismissInput()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        while (IsAnyInputPressed())
            yield return null;

        while (!WasAnyInputPressedThisFrame())
            yield return null;
    }

    private bool IsAnyInputPressed()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.isPressed)
            return true;

        if (Mouse.current != null &&
            (Mouse.current.leftButton.isPressed ||
            Mouse.current.rightButton.isPressed ||
            Mouse.current.middleButton.isPressed))
            return true;

        if (Gamepad.current != null &&
            (Gamepad.current.buttonSouth.isPressed ||   
            Gamepad.current.buttonEast.isPressed ||    
            Gamepad.current.buttonWest.isPressed ||    
            Gamepad.current.buttonNorth.isPressed ||  
            Gamepad.current.startButton.isPressed ||
            Gamepad.current.selectButton.isPressed))
            return true;

        return false;
    }

    private bool WasAnyInputPressedThisFrame()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        if (Mouse.current != null &&
            (Mouse.current.leftButton.wasPressedThisFrame ||
            Mouse.current.rightButton.wasPressedThisFrame ||
            Mouse.current.middleButton.wasPressedThisFrame))
            return true;

        if (Gamepad.current != null &&
            (Gamepad.current.buttonSouth.wasPressedThisFrame ||
            Gamepad.current.buttonEast.wasPressedThisFrame ||
            Gamepad.current.buttonWest.wasPressedThisFrame ||
            Gamepad.current.buttonNorth.wasPressedThisFrame ||
            Gamepad.current.startButton.wasPressedThisFrame ||
            Gamepad.current.selectButton.wasPressedThisFrame))
            return true;

        return false;
    }



    private void RunSpinCheckLogic(SlotStruct slot)
    {
        if (slot == null || !slot.shouldSpin) return;

        NonNormalizedSpringAPI springAPI = slot.springSlot;

        if (springAPI.GetPosition() >= springAPI.goalValue)
        {
            springAPI.ResetPosition();
        }

        springAPI.SetGoalValue(springAPI.goalValue);
    }

    public void StartSpinningSlot1() => StartCoroutine(StartSlotCoroutine(slot1));
    public void StartSpinningSlot2() => StartCoroutine(StartSlotCoroutine(slot2));
    public void StartSpinningSlot3() => StartCoroutine(StartSlotCoroutine(slot3));

    public void StartSpinningAll()
    {
        StartCoroutine(StartSlotCoroutine(slot1));
        StartCoroutine(StartSlotCoroutine(slot2));
        StartCoroutine(StartSlotCoroutine(slot3));
    }

    private IEnumerator StartSlotCoroutine(SlotStruct slot)
    {
        StartSlotSpin(slot);

        float elapsedTime = 0f;
        while (elapsedTime < slot.spinDuration)
        {
            if (!FreezeManager.PauseMenuOverride)
            {
                elapsedTime += Time.unscaledDeltaTime;
            }
            yield return null;
        }

        StopSlotSpin(slot);
    }

    private IEnumerator SlotAudioRoutine()
    {
        AudioManager.Instance?.PlaySFX(leverSfxName);
        AudioManager.Instance?.PlaySFX(spinSfxName);

        yield return new WaitForSecondsRealtime(spinDuration1);
        AudioManager.Instance?.PlaySFX(thumpSfxName);

        yield return new WaitForSecondsRealtime(spinDuration2 - spinDuration1);
        AudioManager.Instance?.PlaySFX(thumpSfxName);

        yield return new WaitForSecondsRealtime(spinDuration3 - spinDuration2);
        AudioManager.Instance?.PlaySFX(thumpSfxName);

        yield return new WaitForSecondsRealtime(0.2f);
        AudioManager.Instance?.PlaySFX(winSfxName);
    }

    private List<int> GenerateRandomInts()
    {
        List<int> randomInts = new List<int>();

        int num1, num2, num3;

        num1 = Random.Range(1, MaxRandomExclusive);
        do { num2 = Random.Range(1, MaxRandomExclusive); } while (num2 == num1);
        do { num3 = Random.Range(1, MaxRandomExclusive); } while (num3 == num1 || num3 == num2);

        randomInts.Add(num1);
        randomInts.Add(num2);
        randomInts.Add(num3);

        return randomInts;
    }
}