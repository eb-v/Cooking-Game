using System.Collections;
using UnityEngine;

public class OilStunEffect : MonoBehaviour
{
    public bool IsStunned { get; private set; }

    private RagdollController ragdollController;
    private float originalBalanceStrength;
    private float originalCoreStrength;
    private float originalLimbStrength;

    private void Awake()
    {
        ragdollController = GetComponent<RagdollController>();
    }

    public void StartStun(float stunDuration, float cooldownDuration, float limpJointStrength)
    {
        if (ragdollController == null) return;

        StartCoroutine(StunCoroutine(stunDuration, cooldownDuration, limpJointStrength));
    }

    private IEnumerator StunCoroutine(float stunDuration, float cooldownDuration, float limpJointStrength)
    {
        IsStunned = true;

        // Disable player movement completely
        ragdollController.TurnMovementOff();

        // Save original joint strengths
        originalBalanceStrength = ragdollController.balanceStrength;
        originalCoreStrength = ragdollController.coreStrength;
        originalLimbStrength = ragdollController.limbStrength;

        // Make player go limp by reducing joint strength
        ragdollController.balanceStrength = limpJointStrength;
        ragdollController.coreStrength = limpJointStrength;
        ragdollController.limbStrength = limpJointStrength;

        // Disable auto-balance and step prediction
        bool originalAutoGetUp = ragdollController.autoGetUpWhenPossible;
        bool originalStepPrediction = ragdollController.useStepPrediction;
        ragdollController.autoGetUpWhenPossible = false;
        ragdollController.useStepPrediction = false;

        // Disable all joint drives to make limbs completely limp
        foreach (var jointPair in ragdollController.RagdollDict)
        {
            RagdollJoint joint = jointPair.Value;
            if (joint != null && joint.Joint != null)
            {
                // Reduce all joint drive forces to make limbs floppy
                JointDrive drive = joint.Joint.slerpDrive;
                drive.positionSpring = limpJointStrength;
                drive.positionDamper = limpJointStrength;
                drive.maximumForce = limpJointStrength;
                joint.Joint.slerpDrive = drive;
            }
        }

        // Wait for stun duration
        yield return new WaitForSeconds(stunDuration);

        // Restore auto-balance and step prediction
        ragdollController.autoGetUpWhenPossible = originalAutoGetUp;
        ragdollController.useStepPrediction = originalStepPrediction;

        // Restore original joint strengths
        ragdollController.balanceStrength = originalBalanceStrength;
        ragdollController.coreStrength = originalCoreStrength;
        ragdollController.limbStrength = originalLimbStrength;

        // Restore joint drives
        foreach (var jointPair in ragdollController.RagdollDict)
        {
            RagdollJoint joint = jointPair.Value;
            if (joint != null && joint.Joint != null)
            {
                JointDrive drive = joint.Joint.slerpDrive;
                drive.positionSpring = originalLimbStrength;
                drive.positionDamper = originalLimbStrength / 10f;
                drive.maximumForce = originalLimbStrength * 10f;
                joint.Joint.slerpDrive = drive;
            }
        }

        // Re-enable player movement
        ragdollController.TurnMovementOn();

        // Wait for cooldown before allowing another hit
        yield return new WaitForSeconds(cooldownDuration);

        IsStunned = false;
    }
}
