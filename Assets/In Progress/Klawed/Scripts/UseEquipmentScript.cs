using UnityEngine;

public class UseEquipmentScript : MonoBehaviour
{
    private bool isUsingEquipment = false;

    void Start()
    {
        GenericEvent<OnEquipmentUseInput>.GetEvent(gameObject.name).AddListener(ChangeUseStatus);
    }

    // Update is called once per frame
    void Update()
    {
        if (isUsingEquipment)
        {
            if (IsPlayerHoldingEquipment())
            {
                IEquipment equipment = GetComponent<GrabScript>().grabbedObject.GetGameObject().GetComponent<IEquipment>();
                equipment.UseEquipment();
            }
        }
    }

    private void ChangeUseStatus(bool status)
    {
        isUsingEquipment = status;
    }

    private bool IsPlayerHoldingEquipment()
    {
        GrabScript gs = GetComponent<GrabScript>();

        if (!gs.IsGrabbing)
        {
            return false;
        }

        return gs.grabbedObject.GetGameObject().GetComponent<IEquipment>() != null;
    }
}
