using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DynamicObjectBase : MonoBehaviour
{
    public bool isGrabbed { get; set; }
    [field:SerializeField] public GrabData grabData { get; set; }

    [field:SerializeField] public Collider grabCollider { get; set; }

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public virtual void GrabObject(GameObject player)
    {
        GrabScript gs = player.GetComponent<GrabScript>();
        if (gs.IsGrabbing || isGrabbed)
            return;


        GrabSystem.GrabObject(player, rb, grabData);
        isGrabbed = true;
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
