using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
// this class is in charge of running scriptable objects which contain ui animation sequences
public class UIAnimManager : MonoBehaviour
{
    // the order in the list determines the order in which the sequences are played
    [SerializeField] private List<UIGroupScript> uiGroupScripts;



    public void StartUISequence()
    {
        StartCoroutine(PlayUIAnimSequence());
    }

    public void ResetUISequence()
    {
        foreach (var group in uiGroupScripts)
        {
            group.springAPI.SetGoalValue(0f);
        }
    }

    private IEnumerator PlayUIAnimSequence()
    {
        foreach (var group in uiGroupScripts)
        {
            group.springAPI.SetGoalValue(1f);
            yield return StartCoroutine(RunDelayCoroutine(group.delay));
        }
    }

    

    private IEnumerator RunDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

}
