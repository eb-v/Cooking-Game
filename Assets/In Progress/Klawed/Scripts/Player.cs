using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(RagdollController))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    #region State Bases
    [Header("Player States")]
    [SerializeField] private BasePlayerState _defaultStateBase;
    [SerializeField] private BasePlayerState _airborneStateBase;
    [SerializeField] private BasePlayerState _lockedStateBase;
    [SerializeField] private BasePlayerState _unconsciousStateBase;
    [SerializeField] private BasePlayerState _animationState;
    [SerializeField] private BasePlayerState _deathStateBase;
    #endregion


    private PlayerStateMachine _stateMachine;

    #region State Instances
    [HideInInspector] public BasePlayerState _defaultStateInstance { get; private set; }
    [HideInInspector] public BasePlayerState _airborneStateInstance { get; private set; }
    [HideInInspector] public BasePlayerState _lockedStateInstance { get; private set; }
    [HideInInspector] public BasePlayerState _unconsciousStateInstance { get; private set; }
    [HideInInspector] public BasePlayerState _animationStateInstance { get; private set; }
    [HideInInspector] public BasePlayerState _deathStateInstance { get; private set; }
    #endregion

    [Header("Debug")]
    [ReadOnly]
    public BasePlayerState _currentState;

    [Header("Cosmetics")]
    [SerializeField] private Image face;
    [ReadOnly]
    [SerializeField] private Material[] materials;
    [ReadOnly]
    [SerializeField] private Sprite faceSprite;

    public Material[] Materials => materials;
    public Sprite FaceSprite => faceSprite;

    private void Awake()
    {
        _stateMachine = new PlayerStateMachine();

        _defaultStateInstance = Instantiate(_defaultStateBase);
        _airborneStateInstance = Instantiate(_airborneStateBase);
        _lockedStateInstance = Instantiate(_lockedStateBase);
        _unconsciousStateInstance = Instantiate(_unconsciousStateBase);
        _animationStateInstance = Instantiate(_animationState);
        _deathStateInstance = Instantiate(_deathStateBase);

    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _defaultStateInstance.Initialize(gameObject, _stateMachine);
        _airborneStateInstance.Initialize(gameObject, _stateMachine);
        _lockedStateInstance.Initialize(gameObject, _stateMachine);
        _unconsciousStateInstance.Initialize(gameObject, _stateMachine);
        _animationStateInstance.Initialize(gameObject, _stateMachine);
        _deathStateInstance.Initialize(gameObject, _stateMachine);

        _stateMachine.Initialize(_defaultStateInstance);
        _currentState = _defaultStateInstance;
    }

    public void ChangeState(BasePlayerState newState)
    {
        _stateMachine.ChangeState(newState);
        _currentState = newState;
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    public void SetToLockedMode(Vector3 pos, Quaternion rot, PoseData poseData)
    {
        RagdollController rc = GetComponent<RagdollController>();
        Transform rootTransform = rc.GetPelvis().gameObject.transform;
        Rigidbody rootRb = rootTransform.GetComponent<Rigidbody>();
        rootRb.isKinematic = true;
        rootTransform.position = pos;
        rootTransform.rotation = rot;
        PoseHelper.SetPlayerPoseAndSnap(rc, poseData);

        foreach (KeyValuePair<string, RagdollJoint> pair in rc.RagdollDict)
        {
            RagdollJoint joint = pair.Value;
            rc.SetJointToOriginalRot(joint);
            if (pair.Key == "LowerRightArm" || pair.Key == "LowerLeftArm")
            {
                if (pair.Value.TryGetComponent<Collider>(out Collider col))
                {
                    col.enabled = false;
                }
            }
            Rigidbody rbJoint = joint.Joint.GetComponent<Rigidbody>();
            rbJoint.linearVelocity = Vector3.zero;
            rbJoint.angularVelocity = Vector3.zero;
        }
        ChangeState(_lockedStateInstance);
    }

    public void SwitchToAnimationMode(RuntimeAnimatorController animController)
    {
        Animator animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animController;
        ChangeState(_animationStateInstance);
    }

    public void Die()
    {
        ChangeState(_deathStateInstance);
    }

    public void SavePlayerCustomization()
    {
        materials = GetComponentInChildren<SkinnedMeshRenderer>().materials;
        faceSprite = face.sprite;
    }

    public void LoadPlayerCustomization(Material[] materials, Sprite faceSprite)
    {
        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.materials = materials;
        }
        face.sprite = faceSprite;
    }


}
