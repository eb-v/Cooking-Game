using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GrabScript : MonoBehaviour
{
    [field:SerializeField] public IGrabable grabbedObject { get; set; }
     public bool isGrabbing => grabbedObject != null;
    
    private void Start()
    {
        //GenericEvent<OnThrowReleased>.GetEvent(gameObject.name).AddListener(PerformThrow);
        GenericEvent<OnObjectGrabbed>.GetEvent(gameObject.name).AddListener(OnObjectGrabbed);
    }
    private void OnObjectGrabbed(IGrabable grabbedObj)
    {
        this.grabbedObject = grabbedObj;
    }

}
