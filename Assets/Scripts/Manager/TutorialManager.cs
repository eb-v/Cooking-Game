using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [SerializeField] private float delayBeforeEnablingNextTutorialZone = 15.0f;

    [Header("Tutorial Steps/states")]
    
    [SerializeField] private List<TutorialState> tutorialStateBases = new List<TutorialState>();
    private List<TutorialState> tutorialStateInstances = new List<TutorialState>();
    private StateMachine<TutorialState> stateMachine;
    private int currentTutorialStateIndex = 0;

    [Header("References")]
    public UDictionary<string, GameObject> tutorialObjectContainers = new UDictionary<string, GameObject>();
    [SerializeField] private GameObject proceedZone;

    #region Singleton
    private static TutorialManager instance;

    public static TutorialManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<TutorialManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("TutorialManager");
                    instance = obj.AddComponent<TutorialManager>();
                }
            }
            return instance;
        }
    }

    #endregion


    private void Awake()
    {
        stateMachine = new StateMachine<TutorialState>();
        foreach (TutorialState tutorialStateBase in tutorialStateBases)
        {
            TutorialState tutorialStateInstance = Instantiate(tutorialStateBase);
            tutorialStateInstance.Initialize(this.gameObject, stateMachine);
            tutorialStateInstances.Add(tutorialStateInstance);
        }
        stateMachine.Initialize(tutorialStateInstances[currentTutorialStateIndex]);
    }

    private void OnEnable()
    {
        GenericEvent<AllTutorialGoalsCompleted>.GetEvent("TutorialManager").AddListener(OnAllTutorialGoalsCompleted);
        GenericEvent<AllPlayersStandingInProceedZone>.GetEvent("TutorialManager").AddListener(GoToNextTutorialStage);
    }

    private void Update()
    {
        stateMachine.GetCurrentState().UpdateLogic();
    }
    private void OnAllTutorialGoalsCompleted()
    {
        proceedZone.SetActive(true);
    }

    private void GoToNextTutorialStage()
    {
        currentTutorialStateIndex++;
        if (currentTutorialStateIndex < tutorialStateInstances.Count)
        {
            stateMachine.ChangeState(tutorialStateInstances[currentTutorialStateIndex]);
        }
        else
        {
            // All tutorial stages completed
            Debug.Log("All tutorial stages completed.");
        }
    }

}
