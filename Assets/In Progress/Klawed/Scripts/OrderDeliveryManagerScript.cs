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
    [SerializeField] private int _maxPizzaOrders = 5;
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

        AddOrder();
    }


    void Update()
    {
        _orderSpawnTimer += Time.deltaTime;
        if (_orderSpawnTimer >= _orderSpawnInterval)
        {
            _orderSpawnTimer = 0f;
            AddOrder();
        }
    }

    public void AddOrder()
    {
        if (OrderLimitNotReached() && canAddOrder)
        {
            /*
            if (PizzaOrderLimitNotReached() && DrinkOrderLimitNotReached())
            {
                int value = Random.Range(0, 2); // 0 for pizza, 1 for drink
                bool shouldSpawnPizza = value == 0;
                FoodOrder newOrder;
                if (shouldSpawnPizza)
                {
                    newOrder = AddPizzaOrder();
                }
                else
                {
                    newOrder = AddDrinkOrder();
                }
                // TODO: Spawn npc with text bubble showing order
                // food sprite is newOrder.foodSprite
            }
            */

            //charlize added >>
            //skips drink check
            if (PizzaOrderLimitNotReached())
            {
                FoodOrder newOrder = AddPizzaOrder();
                Debug.Log("AddOrder called");
            }
        }
    }
    
    // this will probably be invoked by an event when player completes an order with an npc
    public void CompleteOrder(FoodOrder foodOrder)
    {
        
    }

    private FoodOrder AddPizzaOrder()
    {
        FoodOrder pizzaOrder = GetOrderOfType(FoodOrderType.Pizza);
        _currentOrders.Add(pizzaOrder);
        GenericEvent<NewOrderAddedEvent>.GetEvent(_assignedChannel).Invoke(pizzaOrder);
        return pizzaOrder;
    }

    private FoodOrder AddDrinkOrder()
    {
        FoodOrder drinkOrder = GetOrderOfType(FoodOrderType.Drink);
        _currentOrders.Add(drinkOrder);
        GenericEvent<NewOrderAddedEvent>.GetEvent(_assignedChannel).Invoke(drinkOrder);
        return drinkOrder;
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

