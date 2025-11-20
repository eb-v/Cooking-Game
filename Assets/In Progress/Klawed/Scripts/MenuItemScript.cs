using UnityEngine;

public class MenuItemScript : MonoBehaviour, IGrabable, IInteractable
{
    [SerializeField] private MenuItem menuItem;

    public MenuItem MenuItem => menuItem;

    public bool isGrabbed { get; set; }
    [field: SerializeField] public GrabData grabData { get; set; }
    public GameObject currentPlayer { get; set; }

    [field: SerializeField] public Collider grabCollider { get; set; }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
    

    public void OnInteract(GameObject player)
    {
        GrabObject(player);
    }

    public void ReleaseObject(GameObject player)
    {
        GrabSystem.ReleaseObject(player);
        isGrabbed = false;
        currentPlayer = null;
        grabCollider.enabled = true;
    }

    public void GrabObject(GameObject player)
    {
        GrabScript gs = player.GetComponent<GrabScript>();
        if (gs.isGrabbing)
            return;

        PhysicsTransform physicsTransform = gameObject.GetComponent<PhysicsTransform>();
        GrabSystem.GrabObject(player, physicsTransform.physicsTransform.gameObject, grabData);
        GenericEvent<OnObjectGrabbed>.GetEvent(player.name).Invoke(this);
        isGrabbed = true;
        currentPlayer = player;
        grabCollider.enabled = false;
    }
}
