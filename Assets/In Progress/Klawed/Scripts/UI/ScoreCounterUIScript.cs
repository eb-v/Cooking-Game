using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreCounterUIScript : MonoBehaviour
{
    [SerializeField] private List<NonNormalizedSpringAPI> springAPI;
    private int[] digits = new int[4];

    private void Awake()
    {
        GenericEvent<UpdateScoreDisplayEvent>.GetEvent("UpdateScoreDisplayEvent").AddListener(UpdateDisplay);
    }

    private void UpdateDisplay(int score)
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

        digits = digitsList.ToArray();

        for (int i = 0; i < springAPI.Count; i++)
        {
            NonNormalizedSpringAPI digitSpring = springAPI[i];

            digitSpring.SetGoalValue(digits[i]);
        }

    }
}
