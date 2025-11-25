using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "PS_Locked", menuName = "Scriptable Objects/States/PlayerState/Locked")]
public class LockedPlayerState : BasePlayerState
{
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
        if (_rootTransform.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

    }

    public override void Exit()
    {
        base.Exit();

        if (_rootTransform.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }
    }

    public override void RunFixedUpdateLogic()
    {
        base.RunFixedUpdateLogic();
    }

    public override void RunUpdateLogic()
    {
        base.RunUpdateLogic();
        _rc.UpdateTargetRotations();
    }

    private void SetPlayerPoseAndSnap(PoseData poseData)
    {
        
    }

    private void SetPlayerPose(PoseData poseData)
    {
        
    }



}
