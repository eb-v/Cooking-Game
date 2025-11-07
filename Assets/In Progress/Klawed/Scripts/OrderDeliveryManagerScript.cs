using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class OrderDeliveryManagerScript : MonoBehaviour
{
    [SerializeField] private AvailableOrdersSO _availableOrders;


    // Get a random order from the list
    public MenuItem GetRandomOrder()
    {
        MenuItem newOrder = _availableOrders.orderList[Random.Range(0, _availableOrders.orderList.Count)];
        return newOrder;
    }
    

}

