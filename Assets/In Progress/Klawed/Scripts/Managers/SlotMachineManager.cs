using UnityEngine;
using System.Collections;

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
        // Freeze everything EXCEPT the slot machine
        FreezeManager.FreezeGameplay();

        SlotMachineScript slotMachineScript = slotMachine.GetComponentInChildren<SlotMachineScript>();
        SpringAPI slotMachineSpring = slotMachine.GetComponentInChildren<SpringAPI>();

        // Use realtime waits so animation still runs
        yield return new WaitForSecondsRealtime(1.0f);

        slotMachineSpring.SetGoalValue(1f);

        yield return new WaitForSecondsRealtime(startDelay);

        slotMachineScript.StartSlotMachine();
    }

    public void StartEndOfSlotMachineAnimation(GameObject slotMachine, float endDelay)
    {
        StartCoroutine(EndSlotMachineAnimation(slotMachine, endDelay));
    }

    private IEnumerator EndSlotMachineAnimation(GameObject slotMachine, float endDelay)
    {
        SpringAPI slotMachineSpring = slotMachine.GetComponentInChildren<SpringAPI>();

        yield return new WaitForSecondsRealtime(endDelay);

        slotMachineSpring.SetGoalValue(0f);

        yield return new WaitForSecondsRealtime(1.5f);

        // Unfreeze game again after slot finishes
        FreezeManager.UnfreezeGameplay();

        GenericEvent<OnSlotMachineAnimationCompleteEvent>
            .GetEvent("OnSlotMachineAnimationCompleteEvent")
            .Invoke();
    }
}
