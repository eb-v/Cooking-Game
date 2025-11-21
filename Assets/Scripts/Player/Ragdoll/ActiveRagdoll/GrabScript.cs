using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GrabScript : MonoBehaviour
{
    //[SerializeField] private Env_Interaction env_Interaction;
    [SerializeField] private float throwForceMultiplier = 1f;
    [SerializeField] private float maxThrowCharge = 10f;
    [Header("Visual")]
    [SerializeField] private GameObject throwChargeVisual;
    [SerializeField] private Image throwChargeFillImage;

    [ReadOnly]
    [SerializeField] private float throwCharge = 0f;
    [field:SerializeField] public IGrabable grabbedObject { get; set; }
     public bool isGrabbing => grabbedObject != null;
    

    private void Awake()
    {
        
    }

    private void Start()
    {
        //GenericEvent<OnInteractInput>.GetEvent(gameObject.name).AddListener(GrabObject);
        GenericEvent<OnThrowHeld>.GetEvent(gameObject.name).AddListener(ChargeThrow);
        //GenericEvent<OnThrowReleased>.GetEvent(gameObject.name).AddListener(PerformThrow);
        GenericEvent<OnObjectGrabbed>.GetEvent(gameObject.name).AddListener(OnObjectGrabbed);
    }
    private void OnAlternateInteract(GameObject player)
    {
        if (isGrabbing)
        {
        }
    }

    private void ChargeThrow()
    {
        if (!isGrabbing)
            return;

        if (throwCharge < maxThrowCharge)
        {
            throwCharge += Time.deltaTime * throwForceMultiplier;
        }
        else
        {
            throwCharge = maxThrowCharge;
        }
        throwChargeFillImage.fillAmount = throwCharge / maxThrowCharge;
    }

    private void PerformThrow(GameObject player)
    {
        if (!isGrabbing)
            return;

        grabbedObject.ThrowObject(player, throwCharge);

    }

    private void OnObjectGrabbed(IGrabable grabbedObj)
    {
        this.grabbedObject = grabbedObj;
    }

}
