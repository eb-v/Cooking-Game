using UnityEngine;

public class FoodOrderSO : ScriptableObject
{
    public Sprite foodSprite;
    public GameObject orderItemPrefab;
    

    public virtual FoodOrderType GetOrderType()
    {
        return FoodOrderType.None;
    }


}
