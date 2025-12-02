using UnityEditor.Rendering;
using UnityEngine;

public class StoveState : BaseStateSO<StoveState>
{
    protected Stove stove;

    public virtual void InteractLogic(GameObject player, Stove stove) { }

    public virtual void AltInteractLogic(GameObject player) { }
}
