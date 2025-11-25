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
                instance = FindFirstObjectByType<OrderManager>();
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
