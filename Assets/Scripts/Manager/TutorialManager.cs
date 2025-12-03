using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [SerializeField] private float delayBeforeEnablingNextTutorialZone = 15.0f;

    [Header("Tutorial Steps/states")]
    [SerializeField] private TutorialState movement;
    [SerializeField] private TutorialState grabbing;
    [SerializeField] private TutorialState interaction;
    [SerializeField] private TutorialState reconnectingJoints;

    private StateMachine<TutorialState> stateMachine;

    [Header("References")]
    public UDictionary<string, GameObject> tutorialObjectContainers = new UDictionary<string, GameObject>();
    public UDictionary<string, GameObject> tutorialUIContainers = new UDictionary<string, GameObject>();
    [SerializeField] private GameObject nextTutorialZone;

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

    public TutorialState movementInstance;
    public TutorialState grabbingInstance;
    public TutorialState interactionInstance;
    public TutorialState reconnectingJointsInstance;

    private void Awake()
    {
        stateMachine = new StateMachine<TutorialState>();
        movementInstance = Instantiate(movement);
        grabbingInstance = Instantiate(grabbing);
        interactionInstance = Instantiate(interaction);
        reconnectingJointsInstance = Instantiate(reconnectingJoints);

    }

    private void Start()
    {
        movementInstance.Initialize(gameObject, stateMachine);
        grabbingInstance.Initialize(gameObject, stateMachine);
        interactionInstance.Initialize(gameObject, stateMachine);
        reconnectingJointsInstance.Initialize(gameObject, stateMachine);


        stateMachine.Initialize(movementInstance);
    }

    private void Update()
    {
        stateMachine.GetCurrentState().UpdateLogic();
    }


    private IEnumerator TutorialZoneCoroutine()
    {
        nextTutorialZone.SetActive(false);
        yield return new WaitForSeconds(delayBeforeEnablingNextTutorialZone);
        nextTutorialZone.SetActive(true);
    }


    public void StartTutorialZoneCoroutine()
    {
        StartCoroutine(TutorialZoneCoroutine());
    }


}
