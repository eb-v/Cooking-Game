using UnityEngine;
using UnityEngine.UI;

public class ThrowScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject throwMeterObj;
    [SerializeField] private Image throwMeterFillImage;
    [SerializeField] private GrabScript grabScript;
    [SerializeField] private Transform body;
    private float currentThrowForce;
    private bool isCharging = false;

    private PlayerData playerData;


    private void Awake()
    {
        playerData = LoadPlayerData.GetPlayerData();
    }

    private void Start()
    {
        GenericEvent<OnThrowStatusChanged>.GetEvent(gameObject.name).AddListener(ChangeIsChargingStatus);
    }

    private void Update()
    {
        if (grabScript.IsGrabbing)
        {
            if (isCharging)
            {
                ChargeThrow();
            }
        }
    }

    

    public void ChargeThrow()
    {
        if (throwMeterObj.activeSelf == false)
        {
            throwMeterObj.SetActive(true);
        }

        if (currentThrowForce < playerData.MaxThrowForce)
        {
            currentThrowForce += Time.deltaTime * playerData.ThrowChargeRateMultiplier;
        }
        else
        {
            currentThrowForce = playerData.MaxThrowForce;
        }
        throwMeterFillImage.fillAmount = currentThrowForce / playerData.MaxThrowForce;
        Debug.Log("Charging throw: " + currentThrowForce);
    }

    public void PerformThrow()
    {
        if (currentThrowForce < playerData.MinThrowForce)
        {
            currentThrowForce = playerData.MinThrowForce;
        }

        //IGrabable objToThrow = grabScript.grabbedObject;
        Grabable objToThrow = grabScript.grabbedObject;
        objToThrow.Throw(gameObject, currentThrowForce, body.forward); // Added the ThrowObject method call
        grabScript.grabbedObject = null;
        throwMeterFillImage.fillAmount = 0f;
        currentThrowForce = 0f;
        throwMeterObj.SetActive(false);
    }

    private void ChangeIsChargingStatus(bool status)
    {
        isCharging = status;
        if (!isCharging)
        {
            if (grabScript.IsGrabbing)
            {
                PerformThrow();
            }
        }
    }

}
