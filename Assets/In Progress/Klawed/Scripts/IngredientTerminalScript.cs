using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


public class IngredientTerminalScript : MonoBehaviour
{
    [SerializeField] private NonNormalizedSpringAPI displaySpring;
    [SerializeField] private List<GameObject> ingredientCatalog;
    public int currentItemIndex = 0;

    private void Start()
    {
        GenericEvent<DPadInteractEvent>.GetEvent(gameObject.name).AddListener(OnDpadInteract);
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(OrderDelivery);
    }

    private void OnDpadInteract(Vector2 input)
    {
        if (input.x > 0)
        {
            NextItem();
        }
        else if (input.x < 0)
        {
            PreviousItem();
        }
    }


    private void NextItem()
    {
        currentItemIndex++;
        if (currentItemIndex > ingredientCatalog.Count - 1)
        {
            currentItemIndex = 0;
        }
        UpdateDisplay(currentItemIndex);
    }

    private void PreviousItem()
    {
        currentItemIndex--;
        if (currentItemIndex < 0)
        {
            currentItemIndex = ingredientCatalog.Count - 1;
        }
        UpdateDisplay(currentItemIndex);
    }

    private void UpdateDisplay(int itemIndex)
    {
        displaySpring.SetGoalValue(itemIndex);
    }

    public GameObject GetCurrentItem() => ingredientCatalog[currentItemIndex];

    public void OrderDelivery(GameObject player)
    {
        GenericEvent<IngredientOrderedEvent>.GetEvent(gameObject.name).Invoke(GetCurrentItem());
    }


}
