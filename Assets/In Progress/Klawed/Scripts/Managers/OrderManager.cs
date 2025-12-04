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


    //spawn weight
        if (Random.value < 0.6f)
        {
            var foodItems = AvailableOrders.orderList.FindAll(item => 
                item.GetOrderType() != MenuItemType.Drink);
            
            if (foodItems.Count > 0)
                return foodItems[Random.Range(0, foodItems.Count)];
        }
 
        var drinkItems = AvailableOrders.orderList.FindAll(item => 
            item.GetOrderType() == MenuItemType.Drink);
        
        if (drinkItems.Count > 0)
            return drinkItems[Random.Range(0, drinkItems.Count)];

        return AvailableOrders.orderList[Random.Range(0, AvailableOrders.orderList.Count)];
    }
    
}
