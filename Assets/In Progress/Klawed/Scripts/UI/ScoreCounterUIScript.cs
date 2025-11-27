using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections;

public class ScoreCounterUIScript : MonoBehaviour
{
    [SerializeField] private List<NonNormalizedSpringAPI> springAPI;
    [SerializeField] private float spinDuration = 2f;
    //private int[] digits = new int[4];
    [ReadOnly]
    [SerializeField] private bool isSpinning = false;

    [Header("Spring Settings")]
    [Tooltip("Higher values make the spring have more force.")]
    [SerializeField] private float angularFrequency = 7f;
    [Tooltip("Higher values makes the spring have less bounce.")]
    [SerializeField] private float dampingRatio = 0.7f;
    [SerializeField] private float goalValueDuringSpin = 9.5f;


    private void Awake()
    {
        GenericEvent<UpdateScoreDisplayEvent>.GetEvent("UpdateScoreDisplayEvent").AddListener(OnScoreUpdated);
    }
    private void OnScoreUpdated(int score)
    {
        int[] digits = ConvertNumber(score);
        StartCoroutine(DisplayNumber(digits));
    }

    private int[] ConvertNumber(int score)
    {
        List<int> digitsList = new List<int>();

        int digitCount = 0;
        while (score > 0)
        {
            digitsList.Add(score % 10);
            score /= 10;
            digitCount++;
        }


        // Reverse the digits to match the display order
        digitsList.Reverse();

        if (digitCount < 4)
        {
            // Pad with leading zeros to ensure 4 digits
            for (int i = 0; i < 4 - digitCount; i++)
            {
                digitsList.Insert(i, 0);
            }
        }

        int[] digits = digitsList.ToArray();
        if (digits.Length > 4)
        {
            digits = digits.Skip(digits.Length - 4).ToArray();
        }
        return digits;
    }

    private void Update()
    {
        if (isSpinning)
        {
            foreach (var spring in springAPI)
            {
                if (spring.GetPosition() >= spring.goalValue)
                {
                    spring.ResetPosition();
                }
            }
        }
    }
    // spin the digit wheels for a duration before settling on the final number
    private IEnumerator DisplayNumber(int[] digits)
    {
        isSpinning = true;
        SpinDigitWheels();
        yield return new WaitForSeconds(spinDuration);
        isSpinning = false;
        SetDisplayNumber(digits);
    }

    private void SetDisplayNumber(int[] digits)
    {
        for (int i = 0; i < springAPI.Count; i++)
        {
            NonNormalizedSpringAPI digitSpring = springAPI[i];

            digitSpring.SetGoalValue(digits[i]);
        }
    }

    private void SpinDigitWheels()
    {
        foreach (var spring in springAPI)
        {
            spring.SetGoalValue(goalValueDuringSpin);
            spring.ResetPosition();
        }
    }

    public void DebugSpin()
    {
        int randomScore = Random.Range(0, 9999);
        OnScoreUpdated(randomScore);
        Debug.Log("Debug Spin to score: " + randomScore);
    }

    public void SetSpringValues()
    {
        foreach (var spring in springAPI)
        {
            spring.angularFrequency = angularFrequency;
            spring.dampingRatio = dampingRatio;
        }
    }
}
