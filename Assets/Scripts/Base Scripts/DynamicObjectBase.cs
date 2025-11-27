using UnityEngine;

[RequireComponent(typeof(PhysicsTransform))]
public class DynamicObjectBase : MonoBehaviour, IInteractable, IGrabable
{
    public bool isGrabbed { get; set; }
    [field:SerializeField] public GrabData grabData { get; set; }

    [field:SerializeField] public ArmRotationData armRotationData { get; set; }
    public GameObject currentPlayer { get; set; }
    [field:SerializeField] public Collider grabCollider { get; set; }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public virtual void GrabObject(GameObject player)
    {
        GrabScript gs = player.GetComponent<GrabScript>();
        if (gs.IsGrabbing || isGrabbed)
            return;


        PhysicsTransform physicsTransform = gameObject.GetComponent<PhysicsTransform>();
        GrabSystem.GrabObject(player, physicsTransform.physicsTransform.gameObject, grabData, armRotationData);
        GenericEvent<OnObjectGrabbed>.GetEvent(player.name).Invoke(this);

        isGrabbed = true;
        currentPlayer = player;
        grabCollider.enabled = false;
    }

    public virtual void OnInteract(GameObject player)
    {
        GrabObject(player);
    }

    public virtual void ReleaseObject(GameObject player)
    {
        GrabSystem.ReleaseObject(player);
        isGrabbed = false;
        currentPlayer = null;
        grabCollider.enabled = true;
    }

    public virtual void ThrowObject(GameObject player, float throwForce)
    {
        ReleaseObject(player);
        Transform physicsTransform = gameObject.GetComponent<PhysicsTransform>().physicsTransform;
        Rigidbody rb = physicsTransform.GetComponent<Rigidbody>();
        RagdollController rc = player.GetComponent<RagdollController>();
        Vector3 throwDirection = rc.RagdollDict["Body"].transform.forward;

        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
    }

}
