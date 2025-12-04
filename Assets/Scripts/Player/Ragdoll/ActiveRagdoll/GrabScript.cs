using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GrabScript : MonoBehaviour
{
    [SerializeField] private Transform pelvis;
    [SerializeField] private LayerMask grabLayerMask;
    private PlayerData playerSettings;
    private RagdollController rc;

    [field: SerializeField] public Grabable grabbedObject { get; set; }
    public bool IsGrabbing => grabbedObject != null;


    private void Awake()
    {
        playerSettings = LoadPlayerData.GetPlayerData();
        rc = GetComponent<RagdollController>();
    }
    private void OnEnable()
    {
        GenericEvent<OnGrabInputEvent>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnGrabInput);
        GenericEvent<OnJointRemoved>.GetEvent(gameObject.transform.root.GetInstanceID().ToString()).AddListener(MakePlayerReleaseObject);
    }

    private void OnDisable()
    {
        GenericEvent<OnGrabInputEvent>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnGrabInput);
        GenericEvent<OnJointRemoved>.GetEvent(gameObject.transform.root.GetInstanceID().ToString()).RemoveListener(MakePlayerReleaseObject);
    }

    private Grabable GrabRayCastDetection()
    {
        Ray ray = new Ray(pelvis.position, -pelvis.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, playerSettings.InteractionRange, grabLayerMask))
        {
            Grabable grabable = hit.collider.GetComponentInParent<Grabable>();
            return grabable;
        }
        else
        {
            return null;
        }
    }

    private void OnGrabInput()
    {
        if (!rc.MissingArm())
        {
            if (!IsGrabbing)
            {
                Grabable grabable = GrabRayCastDetection();
                if (grabable != null)
                {
                    grabable.Grab(gameObject);
                }
            }
        }
    }
    // Called by outside scripts to force the player to grab the input object
    public void MakePlayerGrabObject(Grabable grabable)
    {
        if (!IsGrabbing)
        {
            grabable.Grab(gameObject);
        }
    }

    public void MakePlayerReleaseObject()
    {
        Debug.Log("MakePlayerReleaseObject called");
        if (IsGrabbing)
        {
            grabbedObject.Release();
        }
    }

}


