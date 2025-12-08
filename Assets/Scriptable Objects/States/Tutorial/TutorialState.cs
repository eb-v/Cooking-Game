using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

[CreateAssetMenu(fileName = "TS_", menuName = "Scriptable Objects/States/Tutorial")]
public class TutorialState : BaseStateSO<TutorialState>
{
    [SerializeField] private List<string> tutorialText;
    [SerializeField] private float tutorialDuration;
    public string objectContainerName;
    public string stateName;

    private void EnableObjects()
    {
        Debug.Log("Enabled objects for tutorial state: " + stateName);
    }

    private void DisableObjects()
    {
    }

    public override void Initialize(GameObject gameObject, StateMachine<TutorialState> stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
    }

    public override void Enter()
    {
        base.Enter();
        DialogueManager.Instance.StartDialogue(tutorialText);
        GenericEvent<OnDialogueFinished>.GetEvent("DialogueManager").AddListener(OnDialogueFinished);
        
    }

    private void OnDialogueFinished()
    {
        EnableObjects();
        CoroutineRunner.Instance.StartCoroutine(EnableProceedZone());
    }

    public override void Exit()
    {
        base.Exit();
        DisableObjects();
        GenericEvent<OnDialogueFinished>.GetEvent("DialogueManager").RemoveListener(OnDialogueFinished);
    }

    private IEnumerator EnableProceedZone()
    {
        yield return new WaitForSeconds(tutorialDuration);
    }
}