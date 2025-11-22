using UnityEngine;

public class ThrowScript : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private float MaxThrowChargeTime;
    private float MinThrowForce;
    private float MaxThrowForce;
    private float ThrowChargeRateMultiplier;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        playerData = LoadPlayerData.GetPlayerData();

        MaxThrowChargeTime = playerData.MaxThrowChargeTime;
        MinThrowForce = playerData.MinThrowForce;
        MaxThrowForce = playerData.MaxThrowForce;
        ThrowChargeRateMultiplier = playerData.ThrowChargeRateMultiplier;
    }

    public void ChargeThrow()
    {
        
    }

    public void PerformThrow()
    {

    }
}
