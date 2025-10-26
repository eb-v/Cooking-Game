using UnityEngine;
using UnityEngine.UI;
public class NpcOrderScript : MonoBehaviour
{
    private FoodOrder _currentFoodOrder;
    [SerializeField] private Image foodImage;

    private void Start()
    {

    }

    public void SetFoodOrder(FoodOrder foodOrder)
    {
        _currentFoodOrder = foodOrder;
        UpdateFoodImage();
    }

    public FoodOrder GetFoodOrder()
    {
        return _currentFoodOrder;
    }

    private void UpdateFoodImage()
    {
        if (foodImage == null)
        {
            Debug.LogWarning("Food image is not assigned.");
            return;
        }

        foodImage.sprite = _currentFoodOrder.foodSprite;
    }
}
