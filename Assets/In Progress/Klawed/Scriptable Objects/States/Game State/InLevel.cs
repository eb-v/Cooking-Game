using UnityEngine;

[CreateAssetMenu(fileName = "InLevel", menuName = "Scriptable Objects/States/Game/InLevel")]
public class InLevel : BaseStateSO
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        // Logic to execute while in level
        Debug.Log("In Level State Executing");
    }

    public override void Exit()
    {
        base.Exit();
    }
}
