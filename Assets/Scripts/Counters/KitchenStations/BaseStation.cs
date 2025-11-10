using UnityEngine;

public class BaseStation : MonoBehaviour, IKitchenStation
{
    private GameObject placedKitchenObj;
    private GameObject currentPlayer;

    
    public virtual void Interact(GameObject player)
    {
        Debug.Log("BaseStation Interact");
    }

    public virtual void RemovePlacedKitchenObj(GameObject player)
    {
        ClearStationObject();
    }

    public virtual void RegisterPlayer(GameObject player)
    {
        currentPlayer = player;
    }

    public virtual void ClearPlayer()
    {
        currentPlayer = null;
    }

    public virtual GameObject GetRegisteredPlayer()
    {
        return currentPlayer;
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



    
