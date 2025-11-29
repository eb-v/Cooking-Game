using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabable : MonoBehaviour
{
    [SerializeField] private GrabData grabData;
    public Collider grabCollider;
    private Rigidbody rb;
    [ReadOnly]
    [SerializeField] private GameObject currentPlayer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabCollider = transform.Find("GrabCollider").GetComponent<Collider>();
        if (grabCollider == null)
        {
            Debug.LogError("GrabCollider container missing from Grabable object: " + gameObject.name);
        }
    }

    public void Grab(GameObject player)
    {
        GrabScript gs = player.GetComponent<GrabScript>();
        gs.grabbedObject = this;
        GrabSystem.GrabObject(player, rb, grabData);
        grabCollider.enabled = false;
        currentPlayer = player;
        GenericEvent<OnObjectGrabbed>.GetEvent(gameObject.GetInstanceID().ToString()).Invoke();
    }

    public void Throw(GameObject player, float force, Vector3 direction)
    {
        GenericEvent<OnObjectThrown>.GetEvent(gameObject.GetInstanceID().ToString()).Invoke();
        Release();
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    public void Release()
    {

        GrabSystem.ReleaseObject(currentPlayer);
        grabCollider.enabled = true;
        currentPlayer = null;
    }

}
