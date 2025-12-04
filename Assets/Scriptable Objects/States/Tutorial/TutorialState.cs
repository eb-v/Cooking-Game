using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

[CreateAssetMenu(fileName = "TS_", menuName = "Scriptable Objects/States/Tutorial")]
public class TutorialState : BaseStateSO<TutorialState>
{
    [SerializeField] private List<string> tutorialText;
    public string objectContainerName;
    public string stateName;

    private void EnableObjects() 
    {
        TutorialManager.Instance.tutorialObjectContainers[stateName].SetActive(true);
    }

    private void DisableObjects() 
    {
        TutorialManager.Instance.tutorialObjectContainers[stateName].SetActive(false);
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
    }

    public override void Exit()
    {
        base.Exit();
        DisableObjects();
    }



}
