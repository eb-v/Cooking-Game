using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Grabable))]
[RequireComponent(typeof(Interactable))]
public class PizzaDoughBase : MonoBehaviour
{
    // for enabling/disabling specific topping visuals on the pizza dough
    [SerializeField] private UDictionary<Ingredient, GameObject> toppingVisuals;


    // to keep track of current toppings on the pizza dough
    private HashSet<Ingredient> currentToppings = new HashSet<Ingredient>();
    private HashSet<Ingredient> validToppings = new HashSet<Ingredient>();

    private void Awake()
    {
        foreach (Ingredient topping in toppingVisuals.Keys)
        {
            validToppings.Add(topping);
        }
    }

    private void OnEnable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnInteract);
    }

    private void OnInteract(GameObject player)
    {
        GrabScript grabScript = player.GetComponent<GrabScript>();
        if (grabScript.IsGrabbing)
        {
            if (PlayerIsHoldingValidTopping(grabScript, out Ingredient heldTopping))
            {
                Grabable heldObject = player.GetComponent<GrabScript>().grabbedObject;
                heldObject.Release();
                Destroy(heldObject.gameObject);


                AddTopping(heldTopping);
            }
            else if (PlayerIsHoldingPotWithTomatoSauce(grabScript, out Ingredient sauce))
            {
                CookingPot cookingPot = grabScript.grabbedObject.GetComponent<CookingPot>();
                cookingPot.RemoveProduct();
                AddTopping(sauce);
            }
        }
    }


    private void AddTopping(Ingredient topping)
    {
        currentToppings.Add(topping);
        if (toppingVisuals.TryGetValue(topping, out GameObject visual))
        {
            visual.SetActive(true);
        }
    }



    #region Topping Detection
    private bool PlayerIsHoldingValidTopping(GrabScript grabScript, out Ingredient topping)
    {
        Grabable heldObject = grabScript.grabbedObject;
        if (IsValidTopping(heldObject))
        {
            IngredientScript ingredientScript = heldObject.GetComponent<IngredientScript>();
            topping = ingredientScript.Ingredient;
            return true;
        }
        topping = null;
        return false;
    }

    private bool IsValidTopping(Grabable grabable)
    {
        if (grabable.TryGetComponent<IngredientScript>(out IngredientScript ingredientScript))
        {
            Ingredient ingredient = ingredientScript.Ingredient;
            if (validToppings.Contains(ingredient))
            {
                return true;
            }
        }
        return false;
    }
    #endregion


    #region Special Case For Adding Sauce

    private bool PlayerIsHoldingPotWithTomatoSauce(GrabScript grabScript, out Ingredient sauce)
    {
        Grabable heldObject = grabScript.grabbedObject;

        if (heldObject.TryGetComponent<CookingPot>(out CookingPot cookingPot))
        {
            if (cookingPot.HasProduct)
            {
                sauce = cookingPot.CurrentProduct;
                return true;
            }
        }
        sauce = null;
        return false;
    }

    #endregion


}
