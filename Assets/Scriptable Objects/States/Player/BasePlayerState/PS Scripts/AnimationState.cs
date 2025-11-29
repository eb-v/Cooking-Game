using UnityEngine;


[CreateAssetMenu(fileName = "PS_Animation", menuName = "Scriptable Objects/States/PlayerState/AnimationState")]
public class AnimationState : BasePlayerState
{
    private RagdollController rc;
    private Animator animator;

    public override void Enter()
    {
        // Disable ragdoll physics and switch to Animation

        // first Switch all rigid bodies to kinematic
        rc.DisablePhysics();

        // Then enable the animator
        animator.enabled = true;
    }

    public override void Exit()
    {
        // Enable ragdoll physics and disable Animation

        // first disable the animator
        animator.enabled = false;

        // Then Switch all rigid bodies to non-kinematic
        rc.EnablePhysics();
    }

    public override void Initialize(GameObject gameObject, PlayerStateMachine stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
        animator = gameObject.GetComponent<Animator>();
        rc = gameObject.GetComponent<RagdollController>();
    }

    public override void RunFixedUpdateLogic()
    {
    }

    public override void RunUpdateLogic()
    {
    }
}
