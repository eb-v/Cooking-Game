using UnityEngine;
using UnityEngine.UI;

public class FoodOrder
{
    public Sprite foodSprite;
    public GameObject orderItemPrefab;
    public FoodOrderType orderType;

    public FoodOrder(Sprite sprite, GameObject orderItemPrefab, FoodOrderType orderType)
    {
        foodSprite = sprite;
        this.orderItemPrefab = orderItemPrefab;
        this.orderType = orderType;
    }

}


public enum FoodOrderType
{
    None,
    Pizza,
    Drink,
    Hamburger
}
