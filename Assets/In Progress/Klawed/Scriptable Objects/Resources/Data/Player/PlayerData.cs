using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    public float MoveSpeed => moveSpeed;

    [SerializeField] private float turnSpeed = 6f;
    public float TurnSpeed => turnSpeed;


    [Header("Balance Settings")]
    [SerializeField] private float groundRayCastLength = 4.0f;
    public float GroundRayCastLength => groundRayCastLength;

    [SerializeField] private float balanceStrength = 5000f;
    public float BalanceStrength => balanceStrength;

    [SerializeField] private float coreStrength = 1500f;
    public float CoreStrength => coreStrength;

    [SerializeField] private float limbStrength = 500f;
    public float LimbStrength => limbStrength;

    [SerializeField] private float stepDuration = 0.22f;
    public float StepDuration => stepDuration;

    [SerializeField] private float stepHeight = 1.7f;
    public float StepHeight => stepHeight;

    [SerializeField] private float feetMountForce = 15.0f;
    public float FeetMountForce => feetMountForce;


    [Header("Knock Out Settings")]
    [SerializeField] private float minKnockOutRecoveryTime = 3.0f;
    public float MinKnockOutRecoveryTime => minKnockOutRecoveryTime;

    [SerializeField] private float maxKnockOutRecoveryTime = 6.0f;
    public float MaxKnockOutRecoveryTime => maxKnockOutRecoveryTime;

    [SerializeField] private float requiredForceToBeKO = 100.0f;
    public float RequiredForceToBeKO => requiredForceToBeKO;


    [Header("Lean Settings")]
    [SerializeField] private float angleMultiplier = 122.5f;
    public float AngleMultiplier => angleMultiplier;


    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 5.0f;
    public float InteractionRange => interactionRange;


    [Header("Throw Settings")]
    [SerializeField] private float maxThrowChargeTime = 2.0f;
    public float MaxThrowChargeTime => maxThrowChargeTime;

    [SerializeField] private float minThrowForce = 10.0f;
    public float MinThrowForce => minThrowForce;

    [SerializeField] private float maxThrowForce = 50.0f;
    public float MaxThrowForce => maxThrowForce;

    [SerializeField] private float throwChargeRateMultiplier = 1.0f;
    public float ThrowChargeRateMultiplier => throwChargeRateMultiplier;


}
