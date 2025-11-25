using UnityEngine;

public class MenuItemScript : DynamicObjectBase
{
    [SerializeField] private MenuItem menuItem;

    public MenuItem MenuItem => menuItem;

    public override void OnInteract(GameObject player)
    {
        base.OnInteract(player);
    }

    public override void ReleaseObject(GameObject player)
    {
        base.ReleaseObject(player);
    }

    public override void GrabObject(GameObject player)
    {
        base.GrabObject(player);
    }

    public override void ThrowObject(GameObject player, float throwForce)
    {
        base.ThrowObject(player, throwForce);
    }
}
