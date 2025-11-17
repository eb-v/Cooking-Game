using UnityEngine;

public class GrabScript : MonoBehaviour
{
    [SerializeField] private Env_Interaction env_Interaction;
    private GameObject lookedAtObj => env_Interaction.currentlyLookingAt;
    private GameObject grabbedObject;
    public bool isGrabbing => grabbedObject != null;
    public GameObject GrabbedObject => grabbedObject;

    private void Awake()
    {
        env_Interaction = GetComponent<Env_Interaction>();
        if (env_Interaction == null)
        {
            Debug.LogError("Env_Interaction component not found on GrabScript GameObject.");
        }
    }

    private void Start()
    {
        GenericEvent<OnInteractInput>.GetEvent(gameObject.name).AddListener(OnInteract);
        GenericEvent<OnAlternateInteractInput>.GetEvent(gameObject.name).AddListener(OnAlternateInteract);
    }

    private void OnInteract(GameObject player)
    {
        if (lookedAtObj != null)
        {
            // if object can be grabbed
            IGrabable grabable = lookedAtObj.GetComponent<IGrabable>();
            if (grabable != null)
            {
                grabable.GrabObject(player);
                grabbedObject = lookedAtObj;
            }
        }
    }

    private void OnAlternateInteract(GameObject player)
    {
        if (isGrabbing)
        {
            GrabSystem.ReleaseObject(player);
        }
    }



}
