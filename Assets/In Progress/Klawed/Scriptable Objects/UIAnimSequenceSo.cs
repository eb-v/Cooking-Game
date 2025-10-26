using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UIAnimSequenceSO", menuName = "Scriptable Objects/UI/UIAnimSequenceSO")]
public class UIAnimSequenceSO : ScriptableObject
{
    // this list holds all springs from all UI elements that will be animated in this sequence
    [SerializeField] private List<SpringAPI> springAPIs;
    // delay before starting the next spring sequence after this one finishes
    public float endDelay = 0f;

    public void Play()
    {
        foreach (SpringAPI spring in springAPIs)
        {
            spring.SetGoalValue(1f);
        }
    }

    public void Reset()
    {
        foreach (SpringAPI spring in springAPIs)
        {
            spring.SetGoalValue(0f);
            spring.ResetSpring();
        }
    }

}

