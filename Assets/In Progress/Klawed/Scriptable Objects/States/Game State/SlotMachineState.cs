using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "SlotMachineState", menuName = "Scriptable Objects/States/Game/SlotMachineState")]
public class SlotMachineState : BaseStateSO
{
    [SerializeField] private GameObject slotMachinePrefab;
    [SerializeField] private UDictionary<LevelModifiers, GameObject> modifierManagers;
    [SerializeField] private float _startDelay = 2.0f;
    private SlotMachineScript slotMachine;
    private GameObject rootManager;


    public override void Enter()
    {
        base.Enter();
        GenericEvent<OnModifiersChoosenEvent>.GetEvent("OnModifiersChoosenEvent").AddListener(OnModifiersChoosen);
        GenericEvent<OnSlotMachineAnimationCompleteEvent>.GetEvent("OnSlotMachineAnimationCompleteEvent").AddListener(OnslotMachineAnimationComplete);

        rootManager = GameObject.Find("Managers");
        if (rootManager == null)
        {
            rootManager = new GameObject("Managers");
        }

        GameObject slotMachineObj = Instantiate(slotMachinePrefab, Vector3.zero, Quaternion.identity);
        slotMachine = slotMachineObj.GetComponent<SlotMachineScript>();


        SlotMachineManager.Instance.StartSlotMachineAnimation(slotMachineObj, _startDelay);
    }


    public override void Execute()
    {
        slotMachine.RunUpdateLogic();
    }

    public override void Exit()
    {
        GenericEvent<OnModifiersChoosenEvent>.GetEvent("OnModifiersChoosenEvent").RemoveListener(OnModifiersChoosen);
        GenericEvent<OnSlotMachineAnimationCompleteEvent>.GetEvent("OnSlotMachineAnimationCompleteEvent").RemoveListener(OnslotMachineAnimationComplete);
        Destroy(slotMachine.gameObject);
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
        
        GameManager.Instance.ChangeState(GameManager.Instance._inLevelStateInstance);
    }
    
}
