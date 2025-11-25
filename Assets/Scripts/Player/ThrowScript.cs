using UnityEngine;
using UnityEngine.UI;

public class ThrowScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject throwMeterObj;
    [SerializeField] private Image throwMeterFillImage;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GrabScript grabScript;
    private float MinThrowForce;
    private float MaxThrowForce;
    private float ThrowChargeRateMultiplier;
    private float currentThrowForce;
    private bool isCharging = false;

    private void Awake()
    {
        Initialize();
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

    private void Initialize()
    {
        playerData = LoadPlayerData.GetPlayerData();

        MinThrowForce = playerData.MinThrowForce;
        MaxThrowForce = playerData.MaxThrowForce;
        ThrowChargeRateMultiplier = playerData.ThrowChargeRateMultiplier;
    }

    public void ChargeThrow()
    {
        if (throwMeterObj.activeSelf == false)
        {
            throwMeterObj.SetActive(true);
        }

        if (currentThrowForce < MaxThrowForce)
        {
            currentThrowForce += Time.deltaTime * ThrowChargeRateMultiplier;
        }
        else
        {
            currentThrowForce = MaxThrowForce;
        }
        throwMeterFillImage.fillAmount = currentThrowForce / MaxThrowForce;
        Debug.Log("Charging throw: " + currentThrowForce);
    }

    public void PerformThrow()
    {
        if (currentThrowForce < MinThrowForce)
        {
            currentThrowForce = MinThrowForce;
        }

        IGrabable objToThrow = grabScript.grabbedObject;
        objToThrow.ThrowObject(gameObject, currentThrowForce);
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
