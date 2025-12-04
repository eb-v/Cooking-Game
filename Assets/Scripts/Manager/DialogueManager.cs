using System.Collections;
using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    private bool isDisplaying = false;
    public bool IsDisplaying => isDisplaying;

    [Header("Dialogue Settings")]
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float dialogueDisplayDuration = 3.0f;
    [Header("References")]
    [SerializeField] private GameObject textBubble;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private DialogueCam dialogueCam;

    private static DialogueManager _instance;

    private int currentLineIndex = 0;
    private List<string> currentDialogueLines = new List<string>();

    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<DialogueManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("DialogueManager");
                    _instance = obj.AddComponent<DialogueManager>();
                }
            }
            return _instance;
        }
    }
    // if called during an active dialogue, it stops the current dialogue and starts the new one
    public void StartDialogue(List<string> dialogue)
    {
        currentDialogueLines = dialogue;
        StopAllCoroutines();
        currentLineIndex = 0;
        if (!textBubble.activeSelf)
        {
            textBubble.SetActive(true);
        }
        dialogueCam.SetCameraActive();
        StartCoroutine(DisplayLine(currentDialogueLines[currentLineIndex]));
    }

    private IEnumerator DisplayLine(string line)
    {

        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        currentLineIndex++;
        if (currentLineIndex >= currentDialogueLines.Count)
        {
            Debug.Log("All dialogue lines displayed.");
            GenericEvent<OnDialogueFinished>.GetEvent("DialogueManager").Invoke();
            dialogueCam.SetCameraInactive();
        }
        else
        {
            StartCoroutine(StartNextLine());
        }
    }

    private IEnumerator StartNextLine()
    {
        yield return new WaitForSeconds(dialogueDisplayDuration);
        StartCoroutine(DisplayLine(currentDialogueLines[currentLineIndex]));
        //GenericEvent<OnDialogueFinished>.GetEvent("DialogueManager").Invoke();

    }


}
