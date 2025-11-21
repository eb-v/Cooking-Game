using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MenuItem", menuName = "Scriptable Objects/Orders/Menu Item")]
public class MenuItem : ScriptableObject
{
    [SerializeField] private Sprite foodSprite;
    [SerializeField] private GameObject orderItemPrefab;
    [SerializeField] private MenuItemType orderType;
    [SerializeField] private List<Ingredient> ingredientsNeeded;
    public List<Ingredient> IngredientsNeeded => ingredientsNeeded;
    public GameObject Prefab => orderItemPrefab;



    public MenuItemType GetOrderType()
    {
        return orderType;
    }

    public Sprite GetFoodSprite()
    {
        return foodSprite;
    }

    public GameObject GetOrderItemPrefab()
    {
        return orderItemPrefab;
    }


}

public enum MenuItemType
{
    None,
    Pizza,
    Drink,
    Hamburger
}

