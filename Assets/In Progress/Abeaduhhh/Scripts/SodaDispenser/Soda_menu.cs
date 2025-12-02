using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Soda_menu : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private Image drinkImage;

    [Header("Available Drinks")]
    [SerializeField] private List<MenuItem> drinkCatalog;
    private string playerEventChannel;

    public int currentDrinkIndex = 0;

    private void Start() {
        GenericEvent<DPadInteractEvent>.GetEvent(transform.parent.name).AddListener(OnDpadInteract);
        GenericEvent<InteractEvent>.GetEvent(transform.parent.name).AddListener(SelectDrink);

        Debug.Log("Got to menu, "+ transform.parent.name);

        UpdateDisplay(currentDrinkIndex);
    }

    private void OnDpadInteract(Vector2 input) {

        Debug.Log("Dpad interactions@@@@@");
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
        PrintCurrentDrink();
    }

    private void PreviousDrink() {
        currentDrinkIndex--;
        if (currentDrinkIndex < 0)
            currentDrinkIndex = drinkCatalog.Count - 1;

        UpdateDisplay(currentDrinkIndex);
        PrintCurrentDrink();
    }

    private void UpdateDisplay(int drinkIndex) {
        if (drinkCatalog == null || drinkCatalog.Count == 0) return;

        MenuItem drink = drinkCatalog[drinkIndex];

        if (drinkImage != null)
            drinkImage.sprite = drink.GetFoodSprite();

    }

    public MenuItem GetCurrentDrink() => drinkCatalog[currentDrinkIndex];

    private void SelectDrink(GameObject player) {
        Debug.Log("SelectDrink triggered! Player: " + player);
        MenuItem drink = GetCurrentDrink();
        if (drink != null) {

            GenericEvent<SodaSelectedEvent>.GetEvent("SodaDispenser").Invoke(drink);
        }
    }

    public void PrintCurrentDrink() {
        MenuItem currentDrink = GetCurrentDrink();
        if (currentDrink != null) {
            Debug.Log("Current drink: " + currentDrink.name);
        } else {
            Debug.Log("No drink selected.");
        }
    }
    public void ListenToPlayer(string playerChannel) {
        if (!string.IsNullOrEmpty(playerEventChannel)) {
            GenericEvent<DPadInteractEvent>.GetEvent(playerEventChannel)
                .RemoveListener(OnDpadInteract);
        }

        playerEventChannel = playerChannel;

        GenericEvent<DPadInteractEvent>.GetEvent(playerChannel)
            .AddListener(OnDpadInteract);

        Debug.Log("Soda menu listening to player channel: " + playerChannel);
    }
    private void OnDisable() {
        if (!string.IsNullOrEmpty(playerEventChannel)) {
            GenericEvent<DPadInteractEvent>.GetEvent(playerEventChannel)
                .RemoveListener(OnDpadInteract);
        }
    }

}
