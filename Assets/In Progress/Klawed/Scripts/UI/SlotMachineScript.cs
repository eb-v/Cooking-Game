using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public enum LevelModifiers
{
    Jetpack,
    OilSpill,
    LandMines,
    LowGravity,
    SpeedBoost,
    ReverseControls,
    test
}

public class SlotMachineScript : MonoBehaviour
{
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

    public void StartSlotMachine()
    {
        int num1, num2, num3;
        num1 = Random.Range(0, 6);
        do { num2 = Random.Range(0, 6); } while (num2 == num1);
        do { num3 = Random.Range(0, 6); } while (num3 == num1 || num3 == num2);

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
        // this value is the goal needed in order to move the spring 1 position past the last visible slot
        // For 6 images available, the goal would be 7 (0-6 visible positions, 7 to loop back to 0)
        // the spring will constantly try to reach this goal value, but once it goes past it, the postion is reset to 0 to create a looping effect
        float spinGoalValue = 7f;
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
        if (slot1.isDone && slot2.isDone && slot3.isDone)
        {
            GenericEvent<OnModifiersChoosenEvent>.GetEvent("OnModifiersChoosenEvent").Invoke(_activeModifiers);

        }
    }

    

    private void RunSpinCheckLogic(SlotStruct slot)
    {
        if (slot.shouldSpin)
        {
            NonNormalizedSpringAPI springAPI = slot.springSlot;
            if (springAPI.GetPosition() >= springAPI.goalValue)
            {
                springAPI.ResetPosition();
            }
        }
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
        yield return new WaitForSeconds(slot.spinDuration);
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

        num1 = Random.Range(0, 6);
        do { num2 = Random.Range(0, 6); } while (num2 == num1);
        do { num3 = Random.Range(0, 6); } while (num3 == num1 || num3 == num2);

        randomInts.Add(num1);
        randomInts.Add(num2);
        randomInts.Add(num3);

        return randomInts;
    }

}
