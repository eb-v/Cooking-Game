using UnityEngine;

[CreateAssetMenu(fileName = "PlayerIdleSO", menuName = "Player/State Behavior Logic/Idle")]
public class PlayerIdleSO : BaseStateSO
{
    private Animator animator;
    private Player player;

    public override void Initialize(GameObject gameObject)
    {
        base.Initialize(gameObject);
        animator = gameObject.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
        player = gameObject.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player component not found on " + gameObject.name);
        }
    }
    
    public override void DoEnterLogic()
    {
        animator.SetTrigger("Idle");
    }

    public override void DoUpdateLogic()
    {
        
    }

    public override void DoFixedUpdateLogic()
    {
        // Implement idle fixed update logic here
    }

    public override void DoExitLogic()
    {
        animator.ResetTrigger("Idle");
    }

    public override void ResetValues()
    {
        // Implement idle reset logic here
    }

    public override void DoAnimationTriggerEventLogic(AnimationTypeEvents type)
    {
        // Implement switch case for animation events if needed
    }
}
