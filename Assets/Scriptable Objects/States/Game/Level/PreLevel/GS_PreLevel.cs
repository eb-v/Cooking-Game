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
        GenericEvent<OnModifiersChoosenEvent>.GetEvent("OnModifiersChoosenEvent").AddListener(OnModifiersChosen);
        GenericEvent<OnSlotMachineAnimationCompleteEvent>.GetEvent("OnSlotMachineAnimationCompleteEvent").AddListener(OnSlotMachineAnimationComplete);

        rootManager = GameObject.Find("Managers");
        if (rootManager == null) {
            rootManager = new GameObject("Managers");
        }

        GameObject slotMachineObj = Instantiate(slotMachinePrefab, Vector3.zero, Quaternion.identity);
        slotMachine = slotMachineObj.GetComponentInChildren<SlotMachineScript>();
        if (slotMachine == null) {
            Debug.LogError("SlotMachineScript not found in prefab hierarchy!");
            return;
        }


        SlotMachineManager.Instance.StartSlotMachineAnimation(slotMachineObj, _startDelay);
        Debug.Log("Entered Pre-Level State");
    }

    public override void Exit()
    {
        base.Exit();
        GenericEvent<OnModifiersChoosenEvent>.GetEvent("OnModifiersChoosenEvent").RemoveListener(OnModifiersChosen);
        GenericEvent<OnSlotMachineAnimationCompleteEvent>.GetEvent("OnSlotMachineAnimationCompleteEvent").RemoveListener(OnSlotMachineAnimationComplete);
        if (slotMachine != null)
            Destroy(slotMachine.gameObject);
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


    private void OnModifiersChosen(List<LevelModifiers> modifiers) {
        SlotMachineManager.Instance.StartEndOfSlotMachineAnimation(slotMachine.gameObject, 1.0f);
    }

    private void OnSlotMachineAnimationComplete() {



        stateMachine.ChangeState(GameManager.Instance._inLevelStateInstance);
    }

}
