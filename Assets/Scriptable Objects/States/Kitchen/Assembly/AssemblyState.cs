using UnityEngine;

public class AssemblyState : BaseStateSO<AssemblyState>
{
    public virtual void InteractLogic(GameObject player) { }
    public virtual void AltInteractLogic(GameObject player) { }
}
