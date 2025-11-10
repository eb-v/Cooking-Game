using UnityEngine;

public interface IKitchenStation
{
    public void ClearStationObject();

    public void SetStationObject(GameObject gameObj);

    public GameObject GetKitchenObject();

    public bool HasKitchenObject();

}
