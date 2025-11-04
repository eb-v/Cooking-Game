using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class SlotMachineScript : MonoBehaviour
{
    private class SlotStruct
    {
        public NonNormalizedSpringAPI springSlot;
        public bool shouldSpin;
        public float spinDuration;
        public int finalGoal;
    }

    [SerializeField] private NonNormalizedSpringAPI springSlot1;
    [SerializeField] private NonNormalizedSpringAPI springSlot2;
    [SerializeField] private NonNormalizedSpringAPI springSlot3;

    [SerializeField] private float spinDuration1 = 2f;
    [SerializeField] private float spinDuration2 = 3f;
    [SerializeField] private float spinDuration3 = 4f;

    private SlotStruct slot1;
    private SlotStruct slot2;
    private SlotStruct slot3;

    private void Awake()
    {
        int num1, num2, num3;

        num1 = Random.Range(0, 6);
        do { num2 = Random.Range(0, 6); } while (num2 == num1);
        do { num3 = Random.Range(0, 6); } while (num3 == num1 || num3 == num2);


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
    }

    private void Update()
    {
        RunSpinCheckLogic(slot1);
        RunSpinCheckLogic(slot2);
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
        RandomizeSlotGoals();

        StartCoroutine(StartSlotCoroutine(slot1));
        StartCoroutine(StartSlotCoroutine(slot2));
        StartCoroutine(StartSlotCoroutine(slot3));

        Debug.Log($"Slot Goals: {slot1.finalGoal}, {slot2.finalGoal}, {slot3.finalGoal}");
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
