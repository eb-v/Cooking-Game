using UnityEngine;
using System.Collections;

public class SlotMachinePanelScript : MonoBehaviour
{
    [SerializeField] private SpringAPI springAPI;
    [SerializeField] private float delayBeforeStart = 2f;
    [SerializeField] private float slotMachineSpinDelay = 1f;
    [SerializeField] private float resumeTimeDelay = 9f;
    [SerializeField] private SlotMachineScript slotMachineScript;

    // reference to existing audio script
    [SerializeField] private SlotMachineAudio slotMachineAudio;

    private void Awake()
    {
        GenericEvent<SlotsFinishedEvent>.GetEvent("SlotMachinePanelScript").AddListener(OnSlotsFinished);

    }

    private void Start()
    {
        //Time.timeScale = 0f;
        StartCoroutine(SlotMachinePosAnimation());
    }

    private IEnumerator SlotMachinePosAnimation()
    {
        yield return new WaitForSecondsRealtime(delayBeforeStart);
        springAPI.SetGoalValue(1f);
        StartCoroutine(StartSpinningSlotMachine());
    }

    private IEnumerator StartSpinningSlotMachine()
    {
        yield return new WaitForSecondsRealtime(slotMachineSpinDelay);

        // audio + spin happen together
        if (slotMachineAudio != null)
        {
            slotMachineAudio.OnSpinButtonClicked();
        }
        else if (slotMachineScript != null)
        {
            slotMachineScript.StartSpinningAll();
        }
    }

    private void OnSlotsFinished()
    {
        StartCoroutine(ContinueGame());
    }

    private IEnumerator ContinueGame()
    {
        yield return new WaitForSecondsRealtime(resumeTimeDelay);
        //Time.timeScale = 1f;
        springAPI.SetGoalValue(0f);
    }

}
