using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private bool isDisplaying = false;
    public bool IsDisplaying => isDisplaying;

    [Header("Dialogue Settings")]
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float dialogueDisplayDuration = 3.0f;
    [SerializeField] private TextMeshProUGUI dialogueText;


    private static DialogueManager _instance;

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

    public void StartDialogue(string line)
    {
        if (isDisplaying)
        {
            StopAllCoroutines();
            isDisplaying = false;
        }

        StartCoroutine(DisplayLine(line));
    }

    private IEnumerator DisplayLine(string line)
    {
        isDisplaying = true; 

        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        StartCoroutine(ResetIsDisplaying());
    }

    private IEnumerator ResetIsDisplaying()
    {
        yield return new WaitForSeconds(dialogueDisplayDuration);
        isDisplaying = false;
        GenericEvent<OnDialogueFinished>.GetEvent("DialogueManager").Invoke();
    }
    

}
