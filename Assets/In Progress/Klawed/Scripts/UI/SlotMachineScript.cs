using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelModifiers {
    None = 0,
    Earthquake = 1,
    Lightning = 2,
    Robber = 3,
    LandMines = 4,
    OilSpill = 5,
    CloseProximity = 6
}

public class SlotMachineScript : MonoBehaviour
{
    private const int MaxVisibleModifierIndex = 6;
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

    private SlotStruct slot1;
    private SlotStruct slot2;
    private SlotStruct slot3;

    [ReadOnly]
    [SerializeField] private List<LevelModifiers> _activeModifiers = new List<LevelModifiers>();

    private void Start()
    {
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
            num1 = Random.Range(1, 7);
            do { num2 = Random.Range(1, 7); } while (num2 == num1);
            do { num3 = Random.Range(1, 7); } while (num3 == num1 || num3 == num2);

            mod1 = (LevelModifiers)num1;
            mod2 = (LevelModifiers)num2;
            mod3 = (LevelModifiers)num3;
        }

        // Start audio sequence
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

        // Start audio sequence
        StartCoroutine(SlotAudioRoutine());
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
        yield return new WaitForSecondsRealtime(resultHoldTime);

        RestoreGameplayCamera();

        FreezeManager.UnfreezeGameplay();

        GenericEvent<OnModifiersChoosenEvent>
            .GetEvent("OnModifiersChoosenEvent")
            .Invoke(_activeModifiers);
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
    // Lever pull + start spin sound
    AudioManager.Instance?.PlaySFX(leverSfxName);
    AudioManager.Instance?.PlaySFX(spinSfxName);

    // Thumps when each reel stops
    yield return new WaitForSeconds(spinDuration1);
    AudioManager.Instance?.PlaySFX(thumpSfxName);

    yield return new WaitForSeconds(spinDuration2 - spinDuration1);
    AudioManager.Instance?.PlaySFX(thumpSfxName);

    yield return new WaitForSeconds(spinDuration3 - spinDuration2);
    AudioManager.Instance?.PlaySFX(thumpSfxName);

    // Small delay, then win sound
    yield return new WaitForSeconds(0.2f);
    AudioManager.Instance?.PlaySFX(winSfxName);
}
    private void RandomizeSlotGoals()
    {
        List<int> randomInts = GenerateRandomInts();
        slot1.finalGoal = randomInts[0];
        slot2.finalGoal = randomInts[1];
        slot3.finalGoal = randomInts[2];
    }

    private void UsePresetSlotGoals()
    {
        slot1.finalGoal = 0;
        slot2.finalGoal = 1;
        slot3.finalGoal = 2;
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
