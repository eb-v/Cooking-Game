using UnityEngine;
using System.Collections;

public class SlotMachinePanelScript : MonoBehaviour
{
    [SerializeField] private SpringAPI springAPI;
    [SerializeField] private float delayBeforeStart = 2f;
    [SerializeField] private float slotMachineSpinDelay = 1f;
    [SerializeField] private float resumeTimeDelay = 9f;
    [SerializeField] private SlotMachineScript slotMachineScript;
    [SerializeField] private Timer timer;
    [SerializeField] private GameObject musicManager;
    [SerializeField] private GameObject BombManager;
    [SerializeField] private BombDropper bombDropper;


    
    
    private void Awake()
    {
        GenericEvent<SlotsFinishedEvent>.GetEvent("SlotMachinePanelScript").AddListener(OnSlotsFinished);

        if (bombDropper == null && BombManager != null)
        {
            bombDropper = BombManager.GetComponent<BombDropper>();
        }
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
        slotMachineScript.StartSpinningAll();
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
        StartCoroutine(DisableObject());
    }

    private IEnumerator DisableObject()
    {
        yield return new WaitForSecondsRealtime(2f);
        gameObject.SetActive(false);
        timer.StartTimer();
        musicManager.SetActive(true);
        BombManager.SetActive(true);

        if (bombDropper != null)
    {
        bombDropper.StartDropping();
    }
}

}
