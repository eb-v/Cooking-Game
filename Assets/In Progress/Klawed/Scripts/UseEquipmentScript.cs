using UnityEngine;

public class UseEquipmentScript : MonoBehaviour
{
    private GrabScript grabScript;
    private bool isUsingEquipment = false;

    private void Awake()
    {
        grabScript = GetComponent<GrabScript>();
    }

    void Start()
    {
        GenericEvent<OnEquipmentUseInput>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(ChangeUseStatus);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (grabScript.IsGrabbing)
        {
            if (grabScript.grabbedObject.TryGetComponent<Equipment>(out Equipment equipment))
            {
                if (isUsingEquipment)
                {
                    equipment.UseEquipment();
                }
            }
        }
    }

    private void ChangeUseStatus(bool status)
    {
        isUsingEquipment = status;
    }

    
}
