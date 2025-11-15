using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SlotMachineManager : MonoBehaviour
{
    private static SlotMachineManager instance;

    public static SlotMachineManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("SlotMachineManager");
                instance = obj.AddComponent<SlotMachineManager>();
                GameObject rootManager = GameObject.Find("Managers");
                if (rootManager == null)
                {
                    rootManager = new GameObject("Managers");
                }
                obj.transform.parent = rootManager.transform;
            }
            return instance;
        }
    }


    public void StartSlotMachineAnimation(GameObject slotMachine, float startDelay)
    {
        StartCoroutine(InitiateSlotMachineAnimation(slotMachine, startDelay));
    }

    public IEnumerator InitiateSlotMachineAnimation(GameObject slotMachine, float startDelay)
    {
        SlotMachineScript slotMachineScript = slotMachine.GetComponentInChildren<SlotMachineScript>();
        SpringAPI slotMachineSpring = slotMachine.GetComponentInChildren<SpringAPI>();
        yield return new WaitForSeconds(1.0f);
        slotMachineSpring.SetGoalValue(1f);
        
        yield return new WaitForSeconds(startDelay);
        slotMachineScript.StartSlotMachine();

    }

    public void StartEndOfSlotMachineAnimation(GameObject slotMachine, float endDelay)
    {
        StartCoroutine(EndSlotMachineAnimation(slotMachine, endDelay));
    }

    private IEnumerator EndSlotMachineAnimation(GameObject slotMachine, float endDelay)
    {
        SpringAPI slotMachineSpring = slotMachine.GetComponentInChildren<SpringAPI>();
        yield return new WaitForSeconds(1.0f);
        slotMachineSpring.SetGoalValue(0f);
        yield return new WaitForSeconds(1.5f);
        GenericEvent<OnSlotMachineAnimationCompleteEvent>.GetEvent("OnSlotMachineAnimationCompleteEvent").Invoke();
    }

}
