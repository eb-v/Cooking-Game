using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "PS_Cutting", menuName = "Scriptable Objects/States/PlayerState/Cutting")]
public class CuttingPlayerState : BasePlayerState
{
    [SerializeField] private PoseData _preChopPose;
    [SerializeField] private PoseData _postChopPose;
    private RagdollController _rc;
    private Transform _rootTransform;

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
        _rc = gameObject.GetComponent<RagdollController>();
        _rootTransform = _rc.GetPelvis().gameObject.transform;
    }


    public override void Enter()
    {
        base.Enter();
        GenericEvent<OnRightTriggerInput>.GetEvent(gameObject.name).AddListener(SetToPostCutPose);
        GenericEvent<OnRightTriggerCancel>.GetEvent(gameObject.name).AddListener(SetToPreCutPose);

        SetPlayerPoseAndSnap(_preChopPose);


        foreach (KeyValuePair<string, RagdollJoint> pair in _rc.RagdollDict)
        {
            RagdollJoint joint = pair.Value;
            _rc.SetJointToOriginalRot(joint);
        }

        Rigidbody rb = _rootTransform.GetComponent<Rigidbody>();
        //rb.isKinematic = true;
    }

    public override void Exit()
    {
        base.Exit();
        GenericEvent<OnRightTriggerInput>.GetEvent(gameObject.name).RemoveListener(SetToPostCutPose);
        GenericEvent<OnRightTriggerCancel>.GetEvent(gameObject.name).RemoveListener(SetToPreCutPose);

        foreach (KeyValuePair<string, RagdollJoint> pair in _rc.RagdollDict)
        {
            RagdollJoint joint = pair.Value;
            _rc.SetJointToOriginalRot(joint);
        }


        Rigidbody rb = _rootTransform.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        _rc.HardResetPose();

    }

    public override void RunFixedUpdateLogic()
    {
        base.RunFixedUpdateLogic();
    }

    public override void RunUpdateLogic()
    {
        base.RunUpdateLogic();
    }

    private void SetPlayerPoseAndSnap(PoseData poseData)
    {
        foreach (KeyValuePair<string, RagdollJoint> kvp in _rc.RagdollDict)
        {
            ConfigurableJoint joint = kvp.Value.Joint;
            joint.targetRotation = poseData.poseJointTargetRotations[kvp.Key];

            PoseHelper.SafeSnapToTarget(joint);
        }
    }

    private void SetPlayerPose(PoseData poseData)
    {
        foreach (KeyValuePair<string, RagdollJoint> kvp in _rc.RagdollDict)
        {
            ConfigurableJoint joint = kvp.Value.Joint;
            joint.targetRotation = poseData.poseJointTargetRotations[kvp.Key];
        }
    }

    private void SetToPostCutPose() => SetPlayerPose(_postChopPose);

    private void SetToPreCutPose() => SetPlayerPose(_preChopPose);


}
