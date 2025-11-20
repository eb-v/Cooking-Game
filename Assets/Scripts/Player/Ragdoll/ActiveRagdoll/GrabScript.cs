using System.Runtime.CompilerServices;
using UnityEngine;

public class GrabScript : MonoBehaviour
{
    [SerializeField] private Env_Interaction env_Interaction;
    //private GameObject lookedAtObj => env_Interaction.currentlyLookingAt;
    [field:SerializeField] public IGrabable grabbedObject { get; set; }
     public bool isGrabbing => grabbedObject != null;
    

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
        //GenericEvent<OnInteractInput>.GetEvent(gameObject.name).AddListener(GrabObject);
        GenericEvent<OnAlternateInteractInput>.GetEvent(gameObject.name).AddListener(OnAlternateInteract);
        GenericEvent<OnObjectGrabbed>.GetEvent(gameObject.name).AddListener(OnObjectGrabbed);
    }
    private void OnAlternateInteract(GameObject player)
    {
        if (isGrabbing)
        {
            grabbedObject.ReleaseObject(player);
        }
    }

    private void OnObjectGrabbed(IGrabable grabbedObj)
    {
        this.grabbedObject = grabbedObj;
    }

}
