using UnityEngine;

[CreateAssetMenu(fileName = "PizzaOrderSO", menuName = "Scriptable Objects/Food Orders/PizzaOrder")]
public class PizzaOrderSO : FoodOrderSO
{
    public override FoodOrderType GetOrderType()
    {
        return FoodOrderType.Pizza;
    }
}
