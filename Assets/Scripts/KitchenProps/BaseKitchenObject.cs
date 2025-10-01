using UnityEngine;

public class BaseKitchenObject : MonoBehaviour {
    [SerializeField] private BaseKitchenObjectSO BaseKitchenObjectSO;

    public BaseKitchenObjectSO GetBaseKitchenObjectSO() {
        return BaseKitchenObjectSO;
    }

    public void DestroySelf()
    {
        Destroy(gameObject); 
    }
}