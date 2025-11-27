using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabable : MonoBehaviour
{
    [SerializeField] private GrabData grabData;
    Collider grabCollider;
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
        GrabSystem.GrabObject(player, rb, grabData);
        grabCollider.enabled = false;
        currentPlayer = player;
    }

    public void Throw(GameObject player, float force, Vector3 direction)
    {
        Release(player);
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    public void Release(GameObject player)
    {
        GrabSystem.ReleaseObject(player);
        grabCollider.enabled = true;
        currentPlayer = null;
    }
}
