using UnityEngine;

[CreateAssetMenu(fileName = "InLobby", menuName = "Scriptable Objects/States/Game/InLobby")]
public class InLobby : BaseStateSO
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        Debug.Log("In Lobby State Executing");
    }

    public override void Exit()
    {
    }
}
