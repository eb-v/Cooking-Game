using UnityEngine;
using UnityEngine.UI;

public class FoodOrder
{
    public Sprite foodSprite;
    public string orderName;

    public FoodOrder(Sprite sprite, string name)
    {
        foodSprite = sprite;
        orderName = name;
    }

}
