using NUnit.Framework;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    private static OrderManager instance;

    public static OrderManager Instance
    {
        get
        {
            if (instance == null)
            {
                if (GameObject.Find("OrderManager") == null)
                {
                    GameObject rootManager = GameObject.Find("Managers");
                    if (rootManager == null)
                    {
                        rootManager = new GameObject("Managers");
                    }
                    instance = new GameObject("OrderManager").AddComponent<OrderManager>();
                    instance.transform.parent = rootManager.transform;
                }
            }
            return instance;
        }
    }

    [SerializeField] private AvailableOrdersSO availableOrders;
    public AvailableOrdersSO AvailableOrders => availableOrders;

    public MenuItem GetRandomOrder()
    {
        if (AvailableOrders == null)
        {
            Debug.LogError("You forgot to assign AvailableOrdersSO in the OrderManager");
            return null;
        }

        if (AvailableOrders.orderList.Count == 0)
        {
            Debug.LogError("AvailableOrdersSO in the Order Manager has no orders in its list.");
            return null;
        }

        return AvailableOrders.orderList[Random.Range(0, AvailableOrders.orderList.Count)];
    }

    


}
