using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public enum LevelModifiers
{
    Jetpack,         // 0
    OilSpill,        // 1
    LandMines,       // 2
    Lightning,       // 3
    Earthquake,      // 4
    Robber,          // 5
    LowGravity,      // 6
    CloseProximity,  // 7
    SpeedBoost,      // 8
    ReverseControls, // 9
    test             // 10
}

public class SlotMachineScript : MonoBehaviour
{
    private const int MaxVisibleModifierIndex = 7;           
    private const int MaxRandomExclusive = MaxVisibleModifierIndex + 1;

    [SerializeField] private bool debugForceModifiers = false;
    [SerializeField] private LevelModifiers[] debugModifiers = new LevelModifiers[3];


    private class SlotStruct
    {
        public NonNormalizedSpringAPI springSlot;
        public bool shouldSpin;
        public float spinDuration;
        public int finalGoal;
        public bool isDone;
    }

    [SerializeField] private NonNormalizedSpringAPI springSlot1;
    [SerializeField] private NonNormalizedSpringAPI springSlot2;
    [SerializeField] private NonNormalizedSpringAPI springSlot3;

    [SerializeField] private float spinDuration1 = 2f;
    [SerializeField] private float spinDuration2 = 3f;
    [SerializeField] private float spinDuration3 = 4f;

    [SerializeField] private bool usePresetGoals = false;

    private SlotStruct slot1;
    private SlotStruct slot2;
    private SlotStruct slot3;

    [ReadOnly]
    [SerializeField] private List<LevelModifiers> _activeModifiers = new List<LevelModifiers>();

    private void Update()
    {
        if (FreezeManager.PauseMenuOverride)
            return;

        RunSpinCheckLogic(slot1);
        RunSpinCheckLogic(slot2);
        RunSpinCheckLogic(slot3);
    }

    public void StartSlotMachine()
    {
        FreezeManager.FreezeGameplay();

        int num1, num2, num3;
        num1 = Random.Range(0, MaxRandomExclusive);                     
        do { num2 = Random.Range(0, MaxRandomExclusive); } while (num2 == num1);
        do { num3 = Random.Range(0, MaxRandomExclusive); } while (num3 == num1 || num3 == num2);

        //testing modifiers
        // if (debugForceModifiers)
        // {
        //     num1 = (int)debugModifiers[0];
        //     num2 = (int)debugModifiers[1];
        //     num3 = (int)debugModifiers[2];
        // }
        // else
        // {
        //     num1 = Random.Range(0, MaxRandomExclusive);
        //     do { num2 = Random.Range(0, MaxRandomExclusive); } while (num2 == num1);
        //     do { num3 = Random.Range(0, MaxRandomExclusive); } while (num3 == num1 || num3 == num2);
        // }



        _activeModifiers.Clear();
        _activeModifiers.Add((LevelModifiers)num1);
        _activeModifiers.Add((LevelModifiers)num2);
        _activeModifiers.Add((LevelModifiers)num3);

        slot1 = new SlotStruct
        {
            springSlot = springSlot1,
            shouldSpin = false,
            spinDuration = spinDuration1,
            finalGoal = num1
        };
        slot2 = new SlotStruct
        {
            springSlot = springSlot2,
            shouldSpin = false,
            spinDuration = spinDuration2,
            finalGoal = num2
        };
        slot3 = new SlotStruct
        {
            springSlot = springSlot3,
            shouldSpin = false,
            spinDuration = spinDuration3,
            finalGoal = num3
        };

        StartSpinningAll();
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

        // spin a little past the last visible index
        float spinGoalValue = MaxRandomExclusive; // 8f
        springAPI.SetGoalValue(spinGoalValue);
        slot.shouldSpin = true;
    }

    private void StopSlotSpin(SlotStruct slot)
    {
        slot.shouldSpin = false;

        NonNormalizedSpringAPI springAPI = slot.springSlot;
        float newGoal = slot.finalGoal;
        springAPI.SetGoalValue(newGoal);
        slot.isDone = true;
        CheckIfAllSlotsDone();
    }

    private void CheckIfAllSlotsDone()
    {
        if (slot1 != null && slot2 != null && slot3 != null &&
            slot1.isDone && slot2.isDone && slot3.isDone)
        {
            // Unfreeze gameplay when slot finishes
            FreezeManager.UnfreezeGameplay();

            GenericEvent<OnModifiersChoosenEvent>
                .GetEvent("OnModifiersChoosenEvent")
                .Invoke(_activeModifiers);
        }
    }

    private void RunSpinCheckLogic(SlotStruct slot)
    {
        if (slot == null || !slot.shouldSpin) return;

        NonNormalizedSpringAPI springAPI = slot.springSlot;

        if (springAPI.GetPosition() >= springAPI.goalValue)
        {
            springAPI.ResetPosition();
        }

        // Ensure spring updates during freeze
        springAPI.SetGoalValue(springAPI.goalValue);
    }

    public void StartSpinningSlot1()
    {
        StartCoroutine(StartSlotCoroutine(slot1));
    }

    public void StartSpinningSlot2()
    {
        StartCoroutine(StartSlotCoroutine(slot2));
    }

    public void StartSpinningSlot3()
    {
        StartCoroutine(StartSlotCoroutine(slot3));
    }

    public void StartSpinningAll()
    {
        if (usePresetGoals)
        {
            UsePresetSlotGoals();
        }
        else
        {
            RandomizeSlotGoals();
        }

        StartCoroutine(StartSlotCoroutine(slot1));
        StartCoroutine(StartSlotCoroutine(slot2));
        StartCoroutine(StartSlotCoroutine(slot3));
    }

    private IEnumerator StartSlotCoroutine(SlotStruct slot)
    {
        StartSlotSpin(slot);

        // Use a manual timer that respects pause state
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

        num1 = Random.Range(0, MaxRandomExclusive);
        do { num2 = Random.Range(0, MaxRandomExclusive); } while (num2 == num1);
        do { num3 = Random.Range(0, MaxRandomExclusive); } while (num3 == num1 || num3 == num2);

        randomInts.Add(num1);
        randomInts.Add(num2);
        randomInts.Add(num3);

        return randomInts;
    }

    //testing visual art
    [ContextMenu("Test Reel1 Art 0..7")]
    private void TestReel1Art()
    {
        StartCoroutine(TestReelArtCoroutine(springSlot1));
    }
    
    private IEnumerator TestReelArtCoroutine(NonNormalizedSpringAPI spring)
    {
        if (spring == null) yield break;

        for (int i = 0; i <= MaxVisibleModifierIndex; i++)
        {
            spring.SetGoalValue(i);
            Debug.Log($"[SlotMachine] Showing symbol index {i} ({(LevelModifiers)i})");
            yield return new WaitForSecondsRealtime(1f);
        }
    }
}
