using UnityEngine;
using System.Collections.Generic;

public class Soda_menu : MonoBehaviour {
    [Header("Display Spring")]
    [SerializeField] private NonNormalizedSpringAPI displaySpring;

    [Header("Available Drinks")]
    [SerializeField] private List<MenuItem> drinkCatalog;

    public int currentDrinkIndex = 0;

    private void Start() {
        GenericEvent<DPadInteractEvent>.GetEvent(gameObject.name).AddListener(OnDpadInteract);
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(SelectDrink);
    }

    private void OnDpadInteract(Vector2 input) {
        if (input.x > 0)
            NextDrink();
        else if (input.x < 0)
            PreviousDrink();
    }

    private void NextDrink() {
        currentDrinkIndex++;

        if (currentDrinkIndex >= drinkCatalog.Count)
            currentDrinkIndex = 0;

        UpdateDisplay(currentDrinkIndex);
    }

    private void PreviousDrink() {
        currentDrinkIndex--;

        if (currentDrinkIndex < 0)
            currentDrinkIndex = drinkCatalog.Count - 1;

        UpdateDisplay(currentDrinkIndex);
    }

    private void UpdateDisplay(int drinkIndex) {
        displaySpring.SetGoalValue(drinkIndex);
    }

    public MenuItem GetCurrentDrink() => drinkCatalog[currentDrinkIndex];

    private void SelectDrink(GameObject player) {
        MenuItem drink = GetCurrentDrink();

        GenericEvent<SodaSelectedEvent>.GetEvent("SodaDispenser")
            .Invoke(drink);

        Debug.Log("Selected drink: " + drink.name);
    }
}
