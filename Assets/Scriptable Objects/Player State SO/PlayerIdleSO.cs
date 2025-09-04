using UnityEngine;

[CreateAssetMenu(fileName = "PlayerIdleSO", menuName = "Player/State Behavior Logic/Idle")]
public class PlayerIdleSO : BaseStateSO
{
    public override void Initialize(GameObject gameObject)
    {
        base.Initialize(gameObject);
        // Additional initialization logic for idle state can be added here
    }
    
    public override void DoEnterLogic()
    {
        // Implement idle enter logic here
    }

    public override void DoUpdateLogic()
    {
        // Implement idle update logic here
    }

    public override void DoFixedUpdateLogic()
    {
        // Implement idle fixed update logic here
    }

    public override void DoExitLogic()
    {
        // Implement idle exit logic here
    }

    public override void ResetValues()
    {
        // Implement idle reset logic here
    }
}
