using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utils;
using System.Collections.Generic;
using JetBrains.Annotations;

public class RagdollController : MonoBehaviour
{

    public UDictionary<string, RagdollJoint> RagdollDict = new UDictionary<string, RagdollJoint>();
    [ReadOnly]
    public UDictionary<string, Quaternion> TargetRotations = new UDictionary<string, Quaternion>();

    public Rigidbody rightHand;
    public Rigidbody leftHand;


    public Transform ObjectSnapPoint;

    public Transform centerOfMass;

    [Header("Save Grab Data Mode")]
    public bool inSaveGrabDataMode = false;
    public Vector3 upperRightArmEuler;
    public Vector3 lowerRightArmEuler;
    public Vector3 upperLeftArmEuler;
    public Vector3 lowerLeftArmEuler;


    [Header("Movement Properties")] public bool forwardIsCameraDirection = true;
    public float moveSpeed = 10f;
    public float turnSpeed = 6f;
    public float jumpForce = 18f;

    public Vector2 MovementAxis { get; set; } = Vector2.zero;
    public Vector2 AimAxis { get; set; }
    public float JumpValue { get; set; } = 0;
    public float GrabLeftValue { get; set; } = 0;
    public float GrabRightValue { get; set; } = 0;

    [Header("Balance Properties")] public bool autoGetUpWhenPossible = true;
    public bool useStepPrediction = true;
    public float balanceHeight = 2.5f;
    public float balanceStrength = 5000f;
    public float coreStrength = 1500f;
    public float limbStrength = 500f;

    public float StepDuration = 0.2f;
    public float StepHeight = 1.7f;
    public float FeetMountForce = 25f;

    public float minKnockOutRecoveryTime = 3f;
    public float maxKnockOutRecoveryTime = 6f;

    [Header("Reach Properties")] public float reachSensitivity = 25f;
    public float armReachStiffness = 2000f;

    [Header("Actions")] public bool canBeKnockoutByImpact = true;
    public float requiredForceToBeKO = 20f;
    public bool canPunch = true;
    public float punchForce = 15f;

    public const string ROOT = "Root";
    public const string BODY = "Body";
    public const string HEAD = "Head";
    public const string UPPER_RIGHT_ARM = "UpperRightArm";
    public const string LOWER_RIGHT_ARM = "LowerRightArm";
    public const string UPPER_LEFT_ARM = "UpperLeftArm";
    public const string LOWER_LEFT_ARM = "LowerLeftArm";
    public const string UPPER_RIGHT_LEG = "UpperRightLeg";
    public const string LOWER_RIGHT_LEG = "LowerRightLeg";
    public const string UPPER_LEFT_LEG = "UpperLeftLeg";
    public const string LOWER_LEFT_LEG = "LowerLeftLeg";
    public const string RIGHT_FOOT = "RightFoot";
    public const string LEFT_FOOT = "LeftFoot";

    //Hidden variables
    private float timer;
    private float Step_R_timer;
    private float Step_L_timer;
    //private float MouseYAxisArms;
    //private float MouseXAxisArms;
    //private float MouseYAxisBody;

    [SerializeField] private bool WalkForward;
    [SerializeField] private bool WalkBackward;
    private bool StepRight;
    private bool StepLeft;
    private bool Alert_Leg_Right;
    private bool Alert_Leg_Left;
    private bool balanced = true;
    private bool GettingUp;
    private bool ResetPose;
    private bool isRagdoll;
    private bool isKeyDown;
    private bool moveAxisUsed;
    private bool jumpAxisUsed;
    private bool leftGrab;
    private bool rightGrab;
    private bool movementToggle = true;
    private bool knockedOut;


    //[SerializeField] private Camera cam;
    private Vector3 Direction;
    private Vector3 CenterOfMassPoint;

    private JointDrive BalanceOn;
    private JointDrive PoseOn;
    private JointDrive CoreStiffness;
    private JointDrive ReachStiffness;
    private JointDrive DriveOff;
    private JointDrive ZeroDrive;
    [ReadOnly]
    [SerializeField] private UDictionary<string, Quaternion> originalTargetRotations;
    private Quaternion HeadTarget;
    private Quaternion BodyTarget;
    private Quaternion UpperRightArmTarget;
    private Quaternion LowerRightArmTarget;
    private Quaternion UpperLeftArmTarget;
    private Quaternion LowerLeftArmTarget;
    private Quaternion UpperRightLegTarget;
    private Quaternion LowerRightLegTarget;
    private Quaternion UpperLeftLegTarget;
    private Quaternion LowerLeftLegTarget;

    private bool _leanForward;
    private bool _leanBackward;
    [Header("Lean Value")]
    [SerializeField] private float _angleMultiplier = 5f;
    private float _leanAngleX = 0f;

    private Dictionary<string, Vector3> originalLocalPositions = new Dictionary<string, Vector3>();
    private Dictionary<string, Quaternion> originalLocalRotations = new Dictionary<string, Quaternion>();

    private static int groundLayer;

    public bool isGrabbing;

    private Quaternion upperRightArmTargetRot = Quaternion.identity;
    private Quaternion lowerRightArmTargetRot = Quaternion.identity;
    private Quaternion upperLeftArmTargetRot = Quaternion.identity;
    private Quaternion lowerLeftArmTargetRot = Quaternion.identity;


    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        PlayerData playerData = LoadPlayerData.GetPlayerData(); 


        moveSpeed = playerData.MoveSpeed;
        turnSpeed = playerData.TurnSpeed;
        balanceHeight = playerData.GroundRayCastLength;
        balanceStrength = playerData.BalanceStrength;
        coreStrength = playerData.CoreStrength;
        limbStrength = playerData.LimbStrength;
        StepDuration = playerData.StepDuration;
        StepHeight = playerData.StepHeight;
        FeetMountForce = playerData.FeetMountForce;
        minKnockOutRecoveryTime = playerData.MinKnockOutRecoveryTime;
        maxKnockOutRecoveryTime = playerData.MaxKnockOutRecoveryTime;
        requiredForceToBeKO = playerData.RequiredForceToBeKO;
        _angleMultiplier = playerData.AngleMultiplier;



        groundLayer = LayerMask.NameToLayer("Ground");
        SetupJointDrives();
        SetupOriginalPose();
        SaveLocalPosRot();

        // Initialize TargetRotations with original target rotations
        foreach (KeyValuePair<string, RagdollJoint> kvp in RagdollDict)
        {
            TargetRotations.Add(kvp.Key, kvp.Value.Joint.targetRotation);
        }

    }

    public void EnterLogic()
    {
        GenericEvent<OnMoveInput>.GetEvent(gameObject.name).AddListener(SetDirection);

        GenericEvent<OnLeanForwardInput>.GetEvent(gameObject.name).AddListener(() => { _leanForward = true; });
        GenericEvent<OnLeanForwardCancel>.GetEvent(gameObject.name).AddListener(() => { _leanForward = false; });
        GenericEvent<OnLeanBackwardInput>.GetEvent(gameObject.name).AddListener(() => { _leanBackward = true; });
        GenericEvent<OnLeanBackwardCancel>.GetEvent(gameObject.name).AddListener(() => { _leanBackward = false; });
        GenericEvent<OnRemoveJoint>.GetEvent(gameObject.name).AddListener(DisconnectJoint);

        GenericEvent<OnGrabStatusChanged>.GetEvent(gameObject.name).AddListener(ChangeGrabStatus);
    }

    public void ExitLogic()
    {
        GenericEvent<OnMoveInput>.GetEvent(gameObject.name).RemoveListener(SetDirection);
        GenericEvent<OnLeanForwardInput>.GetEvent(gameObject.name).RemoveListener(() => { _leanForward = true; });
        GenericEvent<OnLeanForwardCancel>.GetEvent(gameObject.name).RemoveListener(() => { _leanForward = false; });
        GenericEvent<OnLeanBackwardInput>.GetEvent(gameObject.name).RemoveListener(() => { _leanBackward = true; });
        GenericEvent<OnLeanBackwardCancel>.GetEvent(gameObject.name).RemoveListener(() => { _leanBackward = false; });
        GenericEvent<OnRemoveJoint>.GetEvent(gameObject.name).RemoveListener(DisconnectJoint);
        GenericEvent<OnGrabStatusChanged>.GetEvent(gameObject.name).RemoveListener(ChangeGrabStatus);
    }


    private void SetupJointDrives()
    {
        BalanceOn = JointDriveHelper.CreateJointDrive(balanceStrength);
        PoseOn = JointDriveHelper.CreateJointDrive(limbStrength);
        CoreStiffness = JointDriveHelper.CreateJointDrive(coreStrength);
        ReachStiffness = JointDriveHelper.CreateJointDrive(armReachStiffness);
        DriveOff = JointDriveHelper.CreateJointDrive(25);
        ZeroDrive = JointDriveHelper.CreateJointDrive(0);
    }

    private void SetupOriginalPose()
    {
        foreach (KeyValuePair<string, RagdollJoint> kvp in RagdollDict)
        {
            originalTargetRotations.Add(kvp.Key, kvp.Value.Joint.targetRotation);
        }

        
        BodyTarget = GetJointTargetRotation(BODY);
        HeadTarget = GetJointTargetRotation(HEAD);
        UpperRightArmTarget = GetJointTargetRotation(UPPER_RIGHT_ARM);
        LowerRightArmTarget = GetJointTargetRotation(LOWER_RIGHT_ARM);
        UpperLeftArmTarget = GetJointTargetRotation(UPPER_LEFT_ARM);
        LowerLeftArmTarget = GetJointTargetRotation(LOWER_LEFT_ARM);
        UpperRightLegTarget = GetJointTargetRotation(UPPER_RIGHT_LEG);
        LowerRightLegTarget = GetJointTargetRotation(LOWER_RIGHT_LEG);
        UpperLeftLegTarget = GetJointTargetRotation(UPPER_LEFT_LEG);
        LowerLeftLegTarget = GetJointTargetRotation(LOWER_LEFT_LEG);
    }


    
    // save all joints local position and rotation
    private void SaveLocalPosRot()
    {
        foreach (KeyValuePair<string, RagdollJoint> pair in RagdollDict)
        {
            originalLocalPositions.Add(pair.Key, pair.Value.transform.localPosition);
            originalLocalRotations.Add(pair.Key, pair.Value.transform.localRotation);
        }
    }

    public void SetJointToOriginalLocalPosRot(RagdollJoint rj)
    {
        GameObject jointObj = rj.gameObject;
        jointObj.transform.localPosition = originalLocalPositions[rj.GetJointName()];
        jointObj.transform.localRotation = originalLocalRotations[rj.GetJointName()];
    }

    public void SetJointToOriginalRot(RagdollJoint rj)
    {
        if (rj.name == "DEF_Pelvis")
        {
            Debug.Log("Skipping Pelvis");
            return;
        }

        GameObject jointObj = rj.gameObject;

        jointObj.transform.localRotation = originalLocalRotations[rj.GetJointName()];
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Quaternion GetJointTargetRotation(string jointName)
    {
        if (RagdollDict.ContainsKey(jointName))
        {
            return RagdollDict[jointName].Joint.targetRotation;
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public void UpdateLogic()
    {
        if (!InAir)
        {
            if (IsMovementOn() && !isRagdoll)
            {
                PlayerMovement();
            }
        }

        if (!isRagdoll)
        {
            UpdateTargetRotations();
        }

        if (balanced && useStepPrediction)
        {
            PerformStepPrediction();
            UpdateCenterOfMass();
        }

        if (!useStepPrediction)
        {
            ResetWalkCycle();
        }


        UpdateBalanceValue();
        SetDisconnectedJointDrives();


        UpdateCenterOfMass();
    }

    public void FixedUpdateLogic()
    {

        PerformWalking();

        if (IsMovementOn())
        {
            PerformPlayerRotation();
        }
        ResetPlayerPose();

        if (_leanForward && !_leanBackward)
        {
            PerformLeanForward();
        }

        if (_leanBackward && !_leanForward)
        {
            PerformLeanBackward();
        }
    }


    public void UpdateTargetRotations()
    {
        foreach (KeyValuePair<string, RagdollJoint> kvp in RagdollDict)
        {
            kvp.Value.Joint.targetRotation = TargetRotations[kvp.Key];
        }
    }


    public void SetForwardDirection(Vector3 direction)
    {
        centerOfMass.forward = direction;
    }

   

    

    private void PerformPlayerRotation()
    {
        if (!RagdollDict.ContainsKey(ROOT))
            return;

        ConfigurableJoint rootJoint = RagdollDict[ROOT].Joint;

        // Get movement direction in world space
        Vector3 moveDir = new Vector3(MovementAxis.x, 0f, MovementAxis.y);

        if (moveDir.sqrMagnitude > 0.001f) // only rotate if moving
        {
            moveDir.Normalize();

            // Desired rotation
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            centerOfMass.forward = Vector3.Slerp(centerOfMass.forward, moveDir, Time.deltaTime * turnSpeed);

            //Smoothly rotate joint
            //rootJoint.targetRotation = Quaternion.Slerp(
            //    rootJoint.targetRotation,
            //    targetRot,  // Inverse because joints work in local space
            //    Time.deltaTime * turnSpeed
            //);

            TargetRotations[ROOT] = Quaternion.Slerp(
                rootJoint.targetRotation,
                targetRot,  // Inverse because joints work in local space
                Time.deltaTime * turnSpeed
                );



        }
    }

    private void PerformLeanForward()
    {
        if (!RagdollDict.ContainsKey(BODY))
            return;

        ConfigurableJoint _bodyJoint = RagdollDict[BODY].Joint;
        Vector3 axis = Vector3.right;
        _leanAngleX = Mathf.Clamp(_leanAngleX - _angleMultiplier * Time.deltaTime, -80f, 80f);

        Quaternion newTargetRotation = Quaternion.Euler(_leanAngleX, 0f, 0f);
        TargetRotations[BODY] = newTargetRotation;
    }

    private void PerformLeanBackward()
    {
        if (!RagdollDict.ContainsKey(BODY))
            return;

        ConfigurableJoint _bodyJoint = RagdollDict[BODY].Joint;
        Vector3 axis = Vector3.right;
        _leanAngleX = Mathf.Clamp(_leanAngleX + _angleMultiplier * Time.deltaTime, -80f, 80f);

        //Quaternion deltaRotation = Quaternion.AngleAxis(_leanAngleX, axis);
        Quaternion newTargetRotation = Quaternion.Euler(_leanAngleX, 0f, 0f);
        TargetRotations[BODY] = newTargetRotation;
    }


    private void ResetPlayerPose()
    {
        if (ResetPose)
        {
            Debug.Log("Resetting Pose");
            if (RagdollDict[BODY].isConnected)
                TargetRotations[BODY] = originalTargetRotations[BODY];

            if (RagdollDict[UPPER_RIGHT_ARM].isConnected)
                TargetRotations[UPPER_RIGHT_ARM] = originalTargetRotations[UPPER_RIGHT_ARM];

            if (RagdollDict[LOWER_RIGHT_ARM].isConnected)
                TargetRotations[LOWER_RIGHT_ARM] = originalTargetRotations[LOWER_RIGHT_ARM];

            if (RagdollDict[UPPER_LEFT_ARM].isConnected)
                TargetRotations[UPPER_LEFT_ARM] = originalTargetRotations[UPPER_LEFT_ARM];

            if (RagdollDict[LOWER_LEFT_ARM].isConnected)
                TargetRotations[LOWER_LEFT_ARM] = originalTargetRotations[LOWER_LEFT_ARM];

            //MouseYAxisArms = 0;
            ResetPose = false;
        }
    }
    // completely reset the pose of all joints to original position
    public void HardResetPose()
    {
        if (RagdollDict[ROOT].isConnected)
            TargetRotations[ROOT] = originalTargetRotations[ROOT];

        if (RagdollDict[UPPER_RIGHT_ARM].isConnected)
            TargetRotations[UPPER_RIGHT_ARM] = originalTargetRotations[UPPER_RIGHT_ARM];

        if (RagdollDict[LOWER_RIGHT_ARM].isConnected)
            TargetRotations[LOWER_RIGHT_ARM] = originalTargetRotations[LOWER_RIGHT_ARM];

        if (RagdollDict[UPPER_LEFT_ARM].isConnected)
            TargetRotations[UPPER_LEFT_ARM] = originalTargetRotations[UPPER_LEFT_ARM];

        if (RagdollDict[LOWER_LEFT_ARM].isConnected)
            TargetRotations[LOWER_LEFT_ARM] = originalTargetRotations[LOWER_LEFT_ARM];

        if (RagdollDict[HEAD].isConnected)
            TargetRotations[HEAD] = originalTargetRotations[HEAD];

        if (RagdollDict[UPPER_RIGHT_LEG].isConnected)
            TargetRotations[UPPER_RIGHT_LEG] = originalTargetRotations[UPPER_RIGHT_LEG];

        if (RagdollDict[LOWER_RIGHT_LEG].isConnected)
            TargetRotations[LOWER_RIGHT_LEG] = originalTargetRotations[LOWER_RIGHT_LEG];

        if (RagdollDict[UPPER_LEFT_LEG].isConnected)
            TargetRotations[UPPER_LEFT_LEG] = originalTargetRotations[UPPER_LEFT_LEG];

        if (RagdollDict[LOWER_LEFT_LEG].isConnected)
            TargetRotations[LOWER_LEFT_LEG] = originalTargetRotations[LOWER_LEFT_LEG];

        if (RagdollDict[RIGHT_FOOT].isConnected)
            TargetRotations[RIGHT_FOOT] = originalTargetRotations[RIGHT_FOOT];

        if (RagdollDict[LEFT_FOOT].isConnected)
            TargetRotations[LEFT_FOOT] = originalTargetRotations[LEFT_FOOT];

        //MouseYAxisArms = 0;
        ResetPose = false;
    }

    

    


    private void LegCheck()
    {
        //RagdollJoint leftLegJoint = RagdollDict[UPPER_LEFT_LEG];
        //RagdollJoint rightLegJoint = RagdollDict[UPPER_RIGHT_LEG];

        //if (!leftLegJoint.isConnected || !rightLegJoint.isConnected)
        //{
        //    Debug.Log("leg is disconnected");
        //    if (balanced)
        //    {
        //        balanced = false;
        //    }
        //}
    }

    private void UpdateBalanceValue()
    {
        balanced = true;

        if (InAir)
        {
            if (balanced)
            {
                balanced = false;
            }
        }

        if (MissingLeg)
        {
            if (balanced)
            {
                balanced = false;
            }
        }

        if (knockedOut)
        {
            if (balanced)
            {
                balanced = false;
            }
        }

        bool needsStateChange = (balanced == isRagdoll);
        if (!needsStateChange)
            return;

        if (balanced)
        {
            DeactivateRagdoll();
        }
        else
        {
            ActivateRagdoll();
        }

    }

    public void KnockOutPlayer(float force)
    {
        knockedOut = true;



       // StartCoroutine(RecoverFromKnockOut(duration));
    }

    private IEnumerator RecoverFromKnockOut(float duration)
    {
        yield return new WaitForSeconds(duration);
        knockedOut = false;
    }

    //private float CalculateKnockOutDuration(float force)
    //{

    //}

    private bool GroundCheck()
    {
        Transform rootTransform = RagdollDict[ROOT].transform;
        Ray ray = new Ray(rootTransform.position, Vector3.down);
        bool isHittingGround = Physics.Raycast(ray, out _, balanceHeight, 1 << groundLayer);

        Color rayColor = isHittingGround ? Color.green : Color.red;
        Debug.DrawRay(ray.origin, ray.direction * balanceHeight, rayColor);

        return isHittingGround;
    }

    private bool MissingLeg => !RagdollDict[UPPER_LEFT_LEG].isConnected || !RagdollDict[UPPER_RIGHT_LEG].isConnected;

    private bool InAir => !GroundCheck();



    private void SetRagdollState(bool shouldRagdoll, ref JointDrive rootJointDrive, ref JointDrive poseJointDrive,
        bool shouldResetPose)
    {


        isRagdoll = shouldRagdoll;
        balanced = !shouldRagdoll;

        SetJointAngularDrives(ROOT, in rootJointDrive);
        SetJointAngularDrives(HEAD, in poseJointDrive);

        SetJointAngularDrives(UPPER_RIGHT_LEG, in poseJointDrive);
        SetJointAngularDrives(LOWER_RIGHT_LEG, in poseJointDrive);
        SetJointAngularDrives(UPPER_LEFT_LEG, in poseJointDrive);
        SetJointAngularDrives(LOWER_LEFT_LEG, in poseJointDrive);
        SetJointAngularDrives(RIGHT_FOOT, in poseJointDrive);
        SetJointAngularDrives(LEFT_FOOT, in poseJointDrive);

        if (shouldResetPose)
            ResetPose = true;
    }

    private void DeactivateRagdoll() => SetRagdollState(false, ref BalanceOn, ref PoseOn, true);

    public void ActivateRagdoll() => SetRagdollState(true, ref DriveOff, ref DriveOff, false);

    private void SetJointAngularDrives(string jointName, in JointDrive jointDrive)
    {
        if (!RagdollDict.ContainsKey(jointName))
            return;
        RagdollDict[jointName].Joint.angularXDrive = jointDrive;
        RagdollDict[jointName].Joint.angularYZDrive = jointDrive;
    }

    private void PlayerMovement()
    {

        if (forwardIsCameraDirection)
        {
            MoveInCameraDirection();
        }
        else
        {
            MoveInOwnDirection();
        }


    }

    private void SetDirection(Vector2 inputDirection)
    {
        MovementAxis = inputDirection;
    }

    private void MoveInCameraDirection()
    {
        // Direction = RagdollDict[ROOT].transform.rotation * new Vector3(MovementAxis.x, 0.0f, MovementAxis.y);

        // Disable movement if player is missing legs
        //if (!RagdollDict.ContainsKey(UPPER_RIGHT_LEG) || !RagdollDict.ContainsKey(UPPER_LEFT_LEG))
        //    return;

        float newMoveSpeed;
        if (!RagdollDict[UPPER_RIGHT_LEG].isConnected || !RagdollDict[UPPER_LEFT_LEG].isConnected)
        {
            newMoveSpeed = 0f;
        }
        else
        {
            newMoveSpeed = moveSpeed;
        }


        Direction = new Vector3(MovementAxis.x, 0.0f, MovementAxis.y);
        Direction.y = 0f;
        Rigidbody rootRigidbody = RagdollDict[ROOT].Rigidbody;
        var velocity = rootRigidbody.linearVelocity;
        if (RagdollDict[HEAD].isConnected)
        {
            rootRigidbody.linearVelocity = Vector3.Lerp(velocity,
            (Direction * newMoveSpeed) + new Vector3(0, velocity.y, 0), 0.8f);
        }
        else
        {
            rootRigidbody.linearVelocity = Vector3.Lerp(velocity,
            (-Direction * moveSpeed) + new Vector3(0, velocity.y, 0), 0.8f);
        }

        if (MovementAxis.x != 0 || MovementAxis.y != 0 && balanced)
        {
            StartWalkingForward();
        }
        else if (MovementAxis is { x: 0, y: 0 })
        {
            StopWalkingForward();
        }
    }

    private void StartWalkingForward()
    {
        if (!WalkForward && !moveAxisUsed)
        {
            WalkForward = true;
            moveAxisUsed = true;
            isKeyDown = true;
        }
    }

    private void StopWalkingForward()
    {
        if (WalkForward && moveAxisUsed)
        {
            WalkForward = false;
            moveAxisUsed = false;
            isKeyDown = false;
        }
    }

    private void MoveInOwnDirection()
    {
        if (MovementAxis.y != 0)
        {
            var rootRigidbody = RagdollDict[ROOT].Rigidbody;
            var v3 = rootRigidbody.transform.forward * (MovementAxis.y * moveSpeed);
            v3.y = rootRigidbody.linearVelocity.y;
            rootRigidbody.linearVelocity = v3;
        }

        if (MovementAxis.y > 0)
        {
            StartWalkingForwardInOwnDirection();
        }
        else if (MovementAxis.y < 0)
        {
            StartWalkingBackward();
        }
        else
        {
            StopWalking();
        }
    }

    private void StartWalkingForwardInOwnDirection() =>
        SetWalkMovingState(() => (!WalkForward && !moveAxisUsed), true, false, true, true, PoseOn);

    private void StartWalkingBackward() =>
        SetWalkMovingState(() => !WalkBackward && !moveAxisUsed, false, true, true, true, PoseOn);

    private void StopWalking() => SetWalkMovingState(() => WalkForward || WalkBackward && moveAxisUsed, false, false,
        false, false, DriveOff);

    private void SetWalkMovingState(Func<bool> activationCondition, bool walkForwardSetState, bool walkBackwardSetState,
        bool isMoveAxisUsed, bool isKeyCurrentlyDown, in JointDrive legsJointDrive)
    {
        if (activationCondition.Invoke())
        {
            InternalChangeWalkState(walkForwardSetState, walkBackwardSetState, isMoveAxisUsed, isKeyCurrentlyDown,
                in legsJointDrive);
        }
    }

    private void InternalChangeWalkState(bool walkForward, bool walkBackward, bool isMoveAxisUsed,
        bool isKeyCurrentlyDown,
        in JointDrive legsJointDrive)
    {
        WalkForward = walkForward;
        WalkBackward = walkBackward;
        moveAxisUsed = isMoveAxisUsed;
        isKeyDown = isKeyCurrentlyDown;
        if (isRagdoll)
            SetJointAngularDrivesForLegs(in legsJointDrive);
    }
    
    private void SetJointAngularDrivesForLegs(in JointDrive jointDrive)
    {
        SetJointAngularDrives(UPPER_RIGHT_LEG, in jointDrive);
        SetJointAngularDrives(LOWER_RIGHT_LEG, in jointDrive);
        SetJointAngularDrives(UPPER_LEFT_LEG, in jointDrive);
        SetJointAngularDrives(LOWER_LEFT_LEG, in jointDrive);
        SetJointAngularDrives(RIGHT_FOOT, in jointDrive);
        SetJointAngularDrives(LEFT_FOOT, in jointDrive);
    }


    public void ExtendArmsOutward(GrabData grabData)
    {

        if (RagdollDict[UPPER_LEFT_ARM].isConnected && RagdollDict[UPPER_RIGHT_ARM].isConnected)
        {

            TargetRotations[UPPER_LEFT_ARM] = grabData.leftArmPose.upperArmRot;
            TargetRotations[LOWER_LEFT_ARM] = grabData.leftArmPose.lowerArmRot;
            TargetRotations[UPPER_RIGHT_ARM] = grabData.rightArmPose.upperArmRot;
            TargetRotations[LOWER_RIGHT_ARM] = grabData.rightArmPose.lowerArmRot;

            //PoseHelper.SnapJointToTarget(RagdollDict[UPPER_LEFT_ARM].Joint, TargetRotations[UPPER_LEFT_ARM]);
            //PoseHelper.SnapJointToTarget(RagdollDict[LOWER_LEFT_ARM].Joint, TargetRotations[LOWER_LEFT_ARM]);
            //PoseHelper.SnapJointToTarget(RagdollDict[UPPER_RIGHT_ARM].Joint, TargetRotations[UPPER_RIGHT_ARM]);
            //PoseHelper.SnapJointToTarget(RagdollDict[LOWER_RIGHT_ARM].Joint, TargetRotations[LOWER_RIGHT_ARM]);
        }


    }

    public void LowerArms()
    {
        if (RagdollDict[UPPER_LEFT_ARM].isConnected && RagdollDict[UPPER_RIGHT_ARM].isConnected)
        {
            //ConfigurableJoint leftArmJoint = RagdollDict[UPPER_LEFT_ARM].Joint;
            //ConfigurableJoint rightArmJoint = RagdollDict[UPPER_RIGHT_ARM].Joint;

            //Vector3 newTargetRotEuler = new Vector3(0f, 0f, 0f);
            //Quaternion newTargetRotation = Quaternion.Euler(newTargetRotEuler);
            //leftArmJoint.targetRotation = newTargetRotation;
            //rightArmJoint.targetRotation = newTargetRotation;
            //leftUpperArm.targetRotation = Quaternion.identity;
            //leftLowerArm.targetRotation = Quaternion.identity;
            //rightUpperArm.targetRotation = Quaternion.identity;
            //rightLowerArm.targetRotation = Quaternion.identity;
            TargetRotations[UPPER_LEFT_ARM] = Quaternion.identity;
            TargetRotations[LOWER_LEFT_ARM] = Quaternion.identity;
            TargetRotations[UPPER_RIGHT_ARM] = Quaternion.identity;
            TargetRotations[LOWER_RIGHT_ARM] = Quaternion.identity;
        }
        

        //leftArmJoint.targetRotation = Quaternion.Lerp(
        //    leftArmJoint.targetRotation,
        //    newTargetRotation, // Adjust the multiplier as needed
        //    Time.deltaTime * armReachStiffness);

        //rightArmJoint.targetRotation = Quaternion.Lerp(
        //    rightArmJoint.targetRotation,
        //    newTargetRotation, // Adjust the multiplier as needed
        //    Time.deltaTime * armReachStiffness);
    }

    private void ChangeGrabStatus(bool status)
    {
        isGrabbing = status;
    }
    

    private void SetDisconnectedJointDrives()
    {
        foreach (KeyValuePair<string, RagdollJoint> kvp in RagdollDict)
        {
            if (!kvp.Value.isConnected)
            {
                SetJointAngularDrives(kvp.Key, in ZeroDrive);
            }
        }
    }


    private void PerformWalking()
    {
        if (RagdollDict[UPPER_RIGHT_LEG].isConnected == false ||
            RagdollDict[UPPER_LEFT_LEG].isConnected == false)
            return;
        if (InAir)
            return;


        if (WalkForward)
        {
            WalkForwards();
        }

        //if (WalkBackward)
        //{
        //    WalkBackwards();
        //}

        if (StepRight)
        {
            TakeStepRight();
        }
        else
        {
            ResetStepRight();
        }

        if (StepLeft)
        {
            TakeStepLeft();
        }
        else
        {
            ResetStepLeft();
        }
    }

    private void ResetStepLeft() =>
        ResetStep(UPPER_LEFT_LEG, LOWER_LEFT_LEG, in UpperLeftLegTarget, in LowerLeftLegTarget, 7f, 18f);

    private void ResetStepRight() => ResetStep(UPPER_RIGHT_LEG, LOWER_RIGHT_LEG, in UpperRightLegTarget,
        in LowerRightLegTarget, 8f, 17f);

    private void ResetStep(string upperLegLabel,
        string lowerLegLabel,
        in Quaternion upperLegTarget,
        in Quaternion lowerLegTarget,
        float upperLegLerpMultiplier,
        float lowerLegLerpMultiplier)
    {

        TargetRotations[upperLegLabel] = Quaternion.Lerp(
            TargetRotations[upperLegLabel], upperLegTarget,
            upperLegLerpMultiplier * Time.fixedDeltaTime);

        TargetRotations[lowerLegLabel] = Quaternion.Lerp(
            TargetRotations[lowerLegLabel], lowerLegTarget,
            lowerLegLerpMultiplier * Time.fixedDeltaTime);


        Vector3 feetForce = -Vector3.up * (FeetMountForce * Time.deltaTime);

        if (IsJointValid(RagdollDict[RIGHT_FOOT].Joint))
            RagdollDict[RIGHT_FOOT].Rigidbody.AddForce(feetForce, ForceMode.Impulse);

        if (IsJointValid(RagdollDict[LEFT_FOOT].Joint))
            RagdollDict[LEFT_FOOT].Rigidbody.AddForce(feetForce, ForceMode.Impulse);
    }

    public void ResetStepValues()
    {
        ResetWalkCycle();
    }

    private void TakeStepLeft() => TakeStep(ref Step_L_timer, LEFT_FOOT, ref StepLeft, ref StepRight, UPPER_LEFT_LEG,
        LOWER_LEFT_LEG, UPPER_RIGHT_LEG);

    private void TakeStepRight() => TakeStep(ref Step_R_timer, RIGHT_FOOT, ref StepRight, ref StepLeft, UPPER_RIGHT_LEG,
        LOWER_RIGHT_LEG, UPPER_LEFT_LEG);

    private void TakeStep(ref float stepTimer,
        string footLabel,
        ref bool stepFootState,
        ref bool oppositeStepFootState,
        string upperJointLabel,
        string lowerJointLabel,
        string upperOppositeJointLabel)
    {
        if (!RagdollDict[upperJointLabel].isConnected || !RagdollDict[lowerJointLabel].isConnected ||
            !RagdollDict[footLabel].isConnected || !RagdollDict[upperOppositeJointLabel].isConnected)
            return;

        stepTimer += Time.fixedDeltaTime;
        RagdollDict[footLabel].Rigidbody
            .AddForce(-Vector3.up * (FeetMountForce * Time.deltaTime), ForceMode.Impulse);

        var upperLegJoint = RagdollDict[upperJointLabel].Joint;
        var upperLegJointTargetRotation = upperLegJoint.targetRotation;

        var lowerLegJoint = RagdollDict[lowerJointLabel].Joint;
        var lowerLegJointTargetRotation = lowerLegJoint.targetRotation;

        var upperOppositeLegJoint = RagdollDict[upperOppositeJointLabel].Joint;
        var upperOppositeLegJointTargetRotation = upperOppositeLegJoint.targetRotation;

        bool isWalking = WalkForward || WalkBackward;

        if (WalkForward)
        {
            TargetRotations[upperJointLabel] = TargetRotations[upperJointLabel].DisplaceX(0.09f * StepHeight);
            TargetRotations[lowerJointLabel] = TargetRotations[lowerJointLabel].DisplaceX(-0.09f * StepHeight * 2);
            TargetRotations[upperOppositeJointLabel] =
                TargetRotations[upperOppositeJointLabel].DisplaceX(-0.12f * StepHeight / 2);
        }

        if (WalkBackward)
        {
            //TODO: Is this necessary for something? It's multiplying by 0.
            TargetRotations[upperJointLabel] = TargetRotations[upperJointLabel].DisplaceX(-0.00f * StepHeight);
            TargetRotations[lowerJointLabel] = TargetRotations[lowerJointLabel].DisplaceX(-0.07f * StepHeight * 2);
            TargetRotations[upperOppositeJointLabel] = TargetRotations[upperOppositeJointLabel].DisplaceX(0.02f * StepHeight / 2);
        }


        if (stepTimer <= StepDuration)
            return;

        stepTimer = 0;
        stepFootState = false;

        if (isWalking)
        {
            oppositeStepFootState = true;
        }
    }

    private void Walk(
        string forwardFootLabel,
        string backFootLabel,
        ref bool forwardFootState,
        ref bool backFootState,
        ref bool forwardAlertLeg,
        ref bool backAlertLeg)
    {
        if (!RagdollDict.ContainsKey(UPPER_LEFT_LEG) || !RagdollDict.ContainsKey(UPPER_RIGHT_LEG))
            return;

        bool forwardFootIsBehind = RagdollDict[forwardFootLabel].transform.position.z <
                                   RagdollDict[backFootLabel].transform.position.z;
        bool forwardFootIsAhead = RagdollDict[forwardFootLabel].transform.position.z >
                                  RagdollDict[backFootLabel].transform.position.z;

        if (forwardFootIsBehind && !backFootState && !forwardAlertLeg)
        {
            forwardFootState = true;
            forwardAlertLeg = true;
            backAlertLeg = true;
        }

        if (forwardFootIsAhead && !forwardFootState && !backAlertLeg)
        {
            backFootState = true;
            backAlertLeg = true;
            forwardAlertLeg = true;
        }
    }

    private void WalkBackwards() => Walk(LEFT_FOOT, RIGHT_FOOT, ref StepLeft, ref StepRight, ref Alert_Leg_Left,
        ref Alert_Leg_Right);

    private void WalkForwards() => Walk(RIGHT_FOOT, LEFT_FOOT, ref StepRight, ref StepLeft, ref Alert_Leg_Right,
        ref Alert_Leg_Left);

    private void PerformStepPrediction()
    {

        if (!RagdollDict.ContainsKey(UPPER_RIGHT_LEG) || !RagdollDict.ContainsKey(UPPER_LEFT_LEG))
            return;

        if (!WalkForward && !WalkBackward)
        {
            StepRight = false;
            StepLeft = false;
            Step_R_timer = 0;
            Step_L_timer = 0;
            Alert_Leg_Right = false;
            Alert_Leg_Left = false;
        }

        if (centerOfMass.position.z < RagdollDict[RIGHT_FOOT].transform.position.z &&
            centerOfMass.position.z < RagdollDict[LEFT_FOOT].transform.position.z)
        {
            WalkBackward = true;
        }
        else
        {
            if (!isKeyDown)
            {
                WalkBackward = false;
            }

        }


        if (centerOfMass.position.z > RagdollDict[RIGHT_FOOT].transform.position.z &&
            centerOfMass.position.z > RagdollDict[LEFT_FOOT].transform.position.z)
        {
            WalkForward = true;
        }
        else
        {
            if (!isKeyDown)
            {
                WalkForward = false;
            }

        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateCenterOfMass()
    {
        //Be wary of this, it's called up to 2 times per frame
        Vector3 massPositionDisplacement = Vector3.zero;
        float totalMass = 0;

        foreach (var element in RagdollDict)
        {
            if (element.Value.isConnected && element.Value != null)
            {
                var joint = element.Value;
                var mass = joint.Rigidbody.mass;
                massPositionDisplacement += mass * joint.transform.position;
                totalMass += mass;
            }
        }

        CenterOfMassPoint = (massPositionDisplacement / totalMass);
        centerOfMass.position = CenterOfMassPoint;
    }

    private void ResetWalkCycle()
    {
        if (!WalkForward && !WalkBackward)
        {
            StepRight = false;
            StepLeft = false;
            Step_R_timer = 0;
            Step_L_timer = 0;
            Alert_Leg_Right = false;
            Alert_Leg_Left = false;
        }
    }

   

    public Vector3 GetPlayerPosition()
    {
        return centerOfMass.position;
    }
    // this function "disconnects" a joint by unlocking its xyz motions and setting connected body to null
    private void DisconnectJoint(string jointName)
    {
        ConfigurableJoint joint = RagdollDict[jointName].Joint;
        GameObject jointObj = joint.gameObject;

        foreach (ConfigurableJoint cj in jointObj.GetComponentsInChildren<ConfigurableJoint>())
        {
            cj.targetRotation = Quaternion.identity;
        }


        JointBackup jointBackupData = new JointBackup
        {
            parent = joint.transform.parent,
            connectedBody = joint.connectedBody,
            localPosition = joint.transform.localPosition,
            localRotation = joint.transform.localRotation,
            connectedAnchor = joint.connectedAnchor
        };

        joint.transform.parent = null;


        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;

        joint.connectedBody = null;


        foreach (RagdollJoint ragdollJoint in jointObj.GetComponentsInChildren<RagdollJoint>())
        {
            ragdollJoint.isConnected = false;
        }


        GenericEvent<JointRemoved>.GetEvent(gameObject.name).Invoke(joint.gameObject, jointBackupData);
    }

    //public void ConnectJoint(string jointName, GameObject jointObjToConnect)
    //{
    //    if (!RagdollDict.ContainsKey(jointName))
    //    {
    //        jointObjToConnect.transform.parent = this.transform;

    //        SetTagRecursively(jointObjToConnect, "Untagged");

    //        jointObjToConnect.AddComponent<ConfigurableJoint>();

    //        RagdollJoint ragdollJoint = jointObjToConnect.GetComponent<RagdollJoint>();
    //        ragdollJoint.SetConfigurableJoint(jointObjToConnect.GetComponent<ConfigurableJoint>());
    //        RagdollDict.Add(jointName, ragdollJoint);

    //    }
    //}

    private void SetTagRecursively(GameObject obj, string newTag)
    {
        obj.tag = newTag;
        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, newTag);
        }
    }


    public void AddJointToMap(RagdollJoint ragdollJoint)
    {
        if (!RagdollDict.ContainsKey(ragdollJoint.GetJointName()))
        {
            RagdollDict.Add(ragdollJoint.GetJointName(), ragdollJoint);
        }
    }


    public void UpdateJointData(string jointName)
    {
        //switch (jointName)
        //{
        //    case UPPER_LEFT_LEG:
        //        UpperLeftLegTarget 
        //}s
    }

    private bool IsJointValid(ConfigurableJoint joint)
    {
        return joint != null;
    }

    public void ResetReconnectedLimbDrives(string jointName)
    {
        SetJointAngularDrives(jointName, in PoseOn);
    }

    private bool IsMovementOn()
    {
        return movementToggle;
    }

    private void SetMovementStatus(bool status)
    {
        movementToggle = status;
    }

    public void TurnMovementOn() => SetMovementStatus(true);
    public void TurnMovementOff() => SetMovementStatus(false);

    public GameObject GetPelvis() => RagdollDict[ROOT].gameObject;


    public void SetArmRotationValues(Vector3 UpperLeftArmRot, Vector3 LowerLeftArmRot, Vector3 UpperRightArmRot, Vector3 LowerRightArmRot)
    {
        upperRightArmTargetRot = Quaternion.Euler(UpperRightArmRot);
        lowerRightArmTargetRot = Quaternion.Euler(LowerRightArmRot);
        upperLeftArmTargetRot = Quaternion.Euler(UpperLeftArmRot);
        lowerLeftArmTargetRot = Quaternion.Euler(LowerLeftArmRot);
    }




}