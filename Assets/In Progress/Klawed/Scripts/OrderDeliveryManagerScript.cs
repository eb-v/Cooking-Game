using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class OrderDeliveryManagerScript : MonoBehaviour
{
    [SerializeField] private AvailableOrdersSO _availableOrdersSO;

    private List<FoodOrder> _availableOrdersList = new List<FoodOrder>();

    [SerializeField] private string _assignedChannel;

    [Header("Order Settings")]
    [SerializeField] private float _orderSpawnInterval = 10f;
    [SerializeField] private int _maxConcurrentOrders = 5;
    [SerializeField] private int _maxPizzaOrders = 3;
    [SerializeField] private int _maxDrinkOrders = 2;
    private float _orderSpawnTimer;
    [SerializeField] private bool canAddOrder = true;



    private List<FoodOrder> _currentOrders = new List<FoodOrder>();



    private void Awake()
    {
        for (int i = 0; i < _availableOrdersSO.orderList.Count; i++)
        {
            FoodOrder foodOrder = new FoodOrder(
                _availableOrdersSO.orderList[i].foodSprite,
                _availableOrdersSO.orderList[i].orderItemPrefab,
                _availableOrdersSO.orderList[i].GetOrderType()
                );

            _availableOrdersList.Add(foodOrder);
        }
    }

    private void Start()
    {
        GenericEvent<MaxOrderSetEvent>.GetEvent(_assignedChannel).Invoke(_maxConcurrentOrders);
    }




    public void AddOrder()
    {
        if (OrderLimitNotReached() && canAddOrder)
        {
            if (PizzaOrderLimitNotReached() && DrinkOrderLimitNotReached())
            {
                //int value = Random.Range(0, 2); // 0 for pizza, 1 for drink
                //bool shouldSpawnPizza = value == 0;

                //FoodOrder pizzaOrder = GetOrderOfType(FoodOrderType.Pizza);
                //_currentOrders.Add(pizzaOrder);
                //GenericEvent<NewOrderAddedEvent>.GetEvent(_assignedChannel).Invoke(pizzaOrder);

                //if (shouldSpawnPizza)
                //{
                //    FoodOrder pizzaOrder = GetOrderOfType(FoodOrderType.Pizza);
                //    _currentOrders.Add(pizzaOrder);
                //    GenericEvent<NewOrderAddedEvent>.GetEvent(_assignedChannel).Invoke(pizzaOrder);
                //}
                //else
                //{
                //    FoodOrder drinkOrder = GetOrderOfType(FoodOrderType.Drink);
                //    _currentOrders.Add(drinkOrder);
                //    GenericEvent<NewOrderAddedEvent>.GetEvent(_assignedChannel).Invoke(drinkOrder);
                //}


            }
        }
        

    }

    public void CompleteOrder(FoodOrder foodOrder)
    {
        //int index = _currentOrders.IndexOf(foodOrder);
        //if (index != -1)
        //{
        //    _currentOrders.RemoveAt(index);
        //    GenericEvent<OrderCompletedEvent>.GetEvent(_assignedChannel).Invoke(foodOrder);
        //}
        //else
        //{
        //    Debug.Log("Attempted to complete an order that does not exist in current orders.");
        //}
    }


   






    private int GetNumberOfCurrentOrders() => _currentOrders.Count;

    private bool OrderLimitNotReached() => GetNumberOfCurrentOrders() < _maxConcurrentOrders;

    private bool PizzaOrderLimitNotReached()
    {
        int pizzaOrderCount = 0;

        foreach (FoodOrder order in _currentOrders)
        {
            if (order.orderType == FoodOrderType.Pizza)
            {
                pizzaOrderCount++;
            }
        }

        if (pizzaOrderCount < _maxPizzaOrders)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool DrinkOrderLimitNotReached()
    {
        int drinkOrderCount = 0;
        foreach (FoodOrder order in _currentOrders)
        {
            if (order.orderType == FoodOrderType.Drink)
            {
                drinkOrderCount++;
            }
        }
        if (drinkOrderCount < _maxDrinkOrders)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private FoodOrder GetOrderOfType(FoodOrderType orderType)
    {
        FoodOrder order;
        do
        {
            order = _availableOrdersList[Random.Range(0, _availableOrdersList.Count)];
            
        } while (order.orderType != orderType);

        return order;
    }


}

