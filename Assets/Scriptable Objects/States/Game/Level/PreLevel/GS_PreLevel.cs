using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GS_PreLevel", menuName = "Scriptable Objects/States/Game/Level/PreLevel")]
public class GS_PreLevel : GameState
{
    [SerializeField] private GameObject slotMachinePrefab;
    [SerializeField] private UDictionary<LevelModifiers, GameObject> modifierManagers;
    [SerializeField] private float _startDelay = 2.0f;
    private SlotMachineScript slotMachine;
    private GameObject rootManager;
    private float _testTimerDuration = 5.0f;
    private float _timer;


    public override void Enter()
    {
        base.Enter();
        //GenericEvent<OnModifiersChoosenEvent>.GetEvent("OnModifiersChoosenEvent").AddListener(OnModifiersChoosen);
        //GenericEvent<OnSlotMachineAnimationCompleteEvent>.GetEvent("OnSlotMachineAnimationCompleteEvent").AddListener(OnslotMachineAnimationComplete);

        //rootManager = GameObject.Find("Managers");
        //if (rootManager == null)
        //{
        //    rootManager = new GameObject("Managers");
        //}

        //GameObject slotMachineObj = Instantiate(slotMachinePrefab, Vector3.zero, Quaternion.identity);
        //slotMachine = slotMachineObj.GetComponent<SlotMachineScript>();


        //SlotMachineManager.Instance.StartSlotMachineAnimation(slotMachineObj, _startDelay);
        Debug.Log("Entered Pre-Level State");
    }

    public override void Exit()
    {
        base.Exit();
        //GenericEvent<OnModifiersChoosenEvent>.GetEvent("OnModifiersChoosenEvent").RemoveListener(OnModifiersChoosen);
        //GenericEvent<OnSlotMachineAnimationCompleteEvent>.GetEvent("OnSlotMachineAnimationCompleteEvent").RemoveListener(OnslotMachineAnimationComplete);
        //Destroy(slotMachine.gameObject);
        Debug.Log("Exited Pre-Level State");
    }

    public override void FixedUpdateLogic()
    {
        base.FixedUpdateLogic();
    }

    public override void Initialize(GameObject gameObject, StateMachine<GameState> stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
    }

    public override void PerformSceneTransition()
    {
        base.PerformSceneTransition();
    }

    public override void PlaceHolder()
    {
        base.PlaceHolder();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //slotMachine.RunUpdateLogic();
        _timer += Time.deltaTime;
        if (_timer >= _testTimerDuration)
        {
            GameManager.Instance.ChangeState(GameManager.Instance._inLevelStateInstance);
        }

    }


    private void OnModifiersChoosen(List<LevelModifiers> modifierList)
    {
        // spawn all required managers for the modifiers
        foreach (LevelModifiers lvlMod in modifierList)
        {
            if (modifierManagers.ContainsKey(lvlMod))
            {
                GameObject manager = Instantiate(modifierManagers[lvlMod], Vector3.zero, Quaternion.identity);
                manager.transform.parent = rootManager.transform;

            }
        }
        SlotMachineManager.Instance.StartEndOfSlotMachineAnimation(slotMachine.gameObject, 1.0f);
    }

    private void OnslotMachineAnimationComplete()
    {
        stateMachine.ChangeState(GameManager.Instance._inLevelStateInstance);
    }

}
