using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TS_", menuName = "Scriptable Objects/States/Tutorial")]
public class TutorialState : BaseStateSO<TutorialState>
{
    [SerializeField] private List<string> tutorialText;
    private int currentDialogueIndex = 0;
    public string objectContainerName;
    public string uiContainerName;
    public string stateName;

    private void EnableObjects() 
    {
        TutorialManager.Instance.tutorialObjectContainers[stateName].SetActive(true);
        TutorialManager.Instance.tutorialUIContainers[stateName].SetActive(true);
    }

    private void DisableObjects() 
    {
        TutorialManager.Instance.tutorialObjectContainers[stateName].SetActive(false);
        TutorialManager.Instance.tutorialUIContainers[stateName].SetActive(false);
    }

    public override void Initialize(GameObject gameObject, StateMachine<TutorialState> stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
    }

    public override void Enter()
    {
        base.Enter();
        EnableObjects();
        DialogueManager.Instance.StartDialogue(tutorialText[currentDialogueIndex]);
        UpdateDialogue();
        GenericEvent<OnDialogueFinished>.GetEvent("DialogueManager").AddListener(UpdateIndex);
    }

    public override void Exit()
    {
        base.Exit();
        DisableObjects();
        GenericEvent<OnDialogueFinished>.GetEvent("DialogueManager").RemoveListener(UpdateIndex);
        currentDialogueIndex = 0;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        UpdateDialogue();
    }

    private void UpdateDialogue()
    {
        if (!DialogueManager.Instance.IsDisplaying && currentDialogueIndex < tutorialText.Count)
        {
            DialogueManager.Instance.StartDialogue(tutorialText[currentDialogueIndex]);
            Debug.Log("Displaying dialogue: " + tutorialText[currentDialogueIndex]);
        }
    }
    

    private void UpdateIndex()
    {
        currentDialogueIndex++;
    }




}
