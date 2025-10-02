using UnityEngine;

public class BaseStation : MonoBehaviour, IKitchenStation
{
    private GameObject placedKitchenObj;

    
    public virtual void Interact(GameObject player)
    {
        Debug.Log("BaseStation Interact");
    }

    public virtual void RemovePlacedKitchenObj(GameObject player)
    {
        ClearStationObject();
    }

    public void ClearStationObject()
    {
        placedKitchenObj = null;
    }

    public void SetStationObject(GameObject kitchenObj)
    {
        placedKitchenObj = kitchenObj;
    }

    public bool HasKitchenObject()
    {
        return placedKitchenObj != null;
    }

    public GameObject GetKitchenObject()
    {
        return placedKitchenObj;
    }

}



    
