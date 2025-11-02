using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AvailableOrders", menuName = "Scriptable Objects/Orders/AvailableOrders")]
public class AvailableOrdersSO : ScriptableObject
{
    public List<FoodOrderSO> orderList;
}


