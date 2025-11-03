using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderTicketScript : MonoBehaviour
{
    [SerializeField] private Image foodSprite;
    [SerializeField] private TextMeshProUGUI orderText;
    private FoodOrder order;

    public void Initialize(FoodOrder foodOrder)
    {
        foodSprite.sprite = foodOrder.foodSprite;

        gameObject.SetActive(true);
    }

    public FoodOrder GetOrder()
    {
        return order;
    }

    public bool CompareOrder(FoodOrder otherOrder)
    {
        return order.foodSprite == otherOrder.foodSprite;
    }
}
