using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour {
    [SerializeField] private DeliveryManager deliveryManager;
    [SerializeField] private Transform ordersPanel;
    [SerializeField] private GameObject orderCardPrefab;

    private void OnEnable() {
        deliveryManager.OnRecipeListChanged.AddListener(UpdateVisual);
    }

    private void OnDisable() {
        deliveryManager.OnRecipeListChanged.RemoveListener(UpdateVisual);
    }

    private void UpdateVisual() {
        foreach (Transform child in ordersPanel) {
            Destroy(child.gameObject);
        }

        foreach (RecipeSO recipe in deliveryManager.WaitingRecipes) {
            GameObject card = Instantiate(orderCardPrefab, ordersPanel);
            OrderCardUI orderCardUI = card.GetComponent<OrderCardUI>();
            orderCardUI.SetRecipe(recipe);
        }
    }
}
