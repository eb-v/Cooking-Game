using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(Interactable))]
public class PizzaOvenScript : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private HingeJoint ovenDoorHinge;

    [Header("Pizza Oven Settings")]
    [SerializeField] private float cookingDuration = 5f;

    [Header("Visual Indicators")]
    [SerializeField] private Image progressFiller;
    [SerializeField] private Image progressBackgroundBar;

    [ReadOnly]
    [SerializeField] private float currentCookingProgress = 0f;

    [SerializeField] private Transform pizzaPlacePoint;
    [ReadOnly]
    [SerializeField] private bool hasPizzaInside = false;
    [ReadOnly]
    [SerializeField] private GameObject currentPizzaInside = null;
    [ReadOnly]
    [SerializeField] private GameObject currentFinishedPizzaInside = null;
    [ReadOnly]
    [SerializeField] private bool isCooking = false;

    [Header("Possible Pizzas")]
    [SerializeField] private List<PizzaRecipe> pizzaRecipes;

    private Dictionary<MenuItem, List<Ingredient>> possiblePizzas = new Dictionary<MenuItem, List<Ingredient>>();

    [ReadOnly]
    [SerializeField] private MenuItem pizzaToMake = null;

    private void Awake()
    {
        foreach (PizzaRecipe recipe in pizzaRecipes)
        {
            possiblePizzas.Add(recipe.resultingPizza, recipe.requiredToppings);
        }

        OpenDoor();
    }

    private void OnEnable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnAltInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnAltInteract);
    }

    private void OnInteract(GameObject player)
    {
        GrabScript grabScript = player.GetComponent<GrabScript>();
        if (grabScript.IsGrabbing)
        {
            if (!hasPizzaInside)
            {
                if (PlayerIsHoldingPizzaDoughBase(grabScript, out PizzaDoughBase pizzaDough))
                {
                    PlacePizzaInOven(pizzaDough);
                }
            }
        }
    }

    private void OnAltInteract(GameObject player)
    {
        GrabScript grabScript = player.GetComponent<GrabScript>();

        if (!grabScript.IsGrabbing)
        {
            if (hasPizzaInside && !isCooking)
            {
                RemovePizzaFromOven(grabScript);
            }
        }
    }


    private void Update()
    {
        if (!isCooking)
        {
            if (hasPizzaInside)
            {
                if (currentPizzaInside.GetComponent<PizzaDoughBase>() != null)
                {
                    if (PizzaDoughHasCorrectToppings(out MenuItem pizzaToMake))
                    {
                        StartCooking(pizzaToMake);
                    }
                }
            }
        }
        else
        {
            RunCookingLogic();
        }
    }




    #region Placement Logic
    private bool PlayerIsHoldingPizzaDoughBase(GrabScript grabScript, out PizzaDoughBase pizzaDough)
    {
        if (grabScript.grabbedObject.TryGetComponent<PizzaDoughBase>(out pizzaDough))
        {
            return true;
        }
        pizzaDough = null;
        return false;
    }

    private void PlacePizzaInOven(PizzaDoughBase pizzaDough)
    {
        Grabable grabable = pizzaDough.GetComponent<Grabable>();
        grabable.Release();
        grabable.grabCollider.enabled = false;

        Interactable interactable = pizzaDough.GetComponent<Interactable>();
        interactable.enabled = false;


        Rigidbody rb = grabable.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        pizzaDough.transform.position = pizzaPlacePoint.position;
        pizzaDough.transform.rotation = pizzaPlacePoint.rotation;

        hasPizzaInside = true;
        currentPizzaInside = pizzaDough.gameObject;
    }

    private void RemovePizzaFromOven(GrabScript grabScript)
    {
        Rigidbody rb = currentPizzaInside.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        Grabable grabable = currentPizzaInside.GetComponent<Grabable>();
        grabScript.MakePlayerGrabObject(grabable);
        currentPizzaInside = null;
        hasPizzaInside = false;
    }
    #endregion

    #region Topping Checking Logic
    private bool PizzaDoughHasCorrectToppings(out MenuItem pizzaToMake)
    {
        foreach (KeyValuePair<MenuItem, List<Ingredient>> kvp in possiblePizzas)
        {
            MenuItem pizza = kvp.Key;
            List<Ingredient> requiredToppings = kvp.Value;
            if (CheckPizzaToppingsMatch(requiredToppings))
            {
                pizzaToMake = pizza;
                return true;
            }
        }

        pizzaToMake = null;
        return false;
    }

    private bool CheckPizzaToppingsMatch(List<Ingredient> requiredToppings)
    {
        foreach (Ingredient topping in requiredToppings)
        {
            if (!currentPizzaInside.GetComponent<PizzaDoughBase>().CurrentToppings.Contains(topping))
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    private void StartCooking(MenuItem pizzaToMake)
    {
        isCooking = true;
        this.pizzaToMake = pizzaToMake;
        EnableProgressBar();
        CloseDoor();
    }

    private void RunCookingLogic()
    {
        currentCookingProgress += Time.deltaTime;
        UpdateProgressVisual();

        if (currentCookingProgress >= cookingDuration)
        {
            DestroyPizzaDoughBase();
            SpawnFinishedPizza();
            StopCooking();
        }
    }

    private void UpdateProgressVisual()
    {
        float progress = currentCookingProgress / cookingDuration;
        progressFiller.fillAmount = progress;
    }

    private void EnableProgressBar()
    {
        progressBackgroundBar.enabled = true;
        progressFiller.enabled = true;
    }

    private void DisableProgressBar()
    {
        progressBackgroundBar.enabled = false;
        progressFiller.enabled = false;
    }

    private void SpawnFinishedPizza()
    {
        GameObject finishedPizzaPrefab = pizzaToMake.Prefab;
        GameObject finishedPizza = Instantiate(finishedPizzaPrefab, pizzaPlacePoint.position, pizzaPlacePoint.rotation);
        currentPizzaInside = finishedPizza;
        hasPizzaInside = true;
    }

    private void DestroyPizzaDoughBase()
    {
        if (currentPizzaInside != null)
        {
            Destroy(currentPizzaInside.gameObject);
            currentPizzaInside = null;
            hasPizzaInside = false;
        }
    }

    private void StopCooking()
    {
        isCooking = false;
        currentCookingProgress = 0f;
        DisableProgressBar();
        OpenDoor();
    }

    private void OpenDoor()
    {
        float openAngle = -90f;
        JointSpring hingeSpring = ovenDoorHinge.spring;
        hingeSpring.targetPosition = openAngle;
        ovenDoorHinge.spring = hingeSpring;
    }

    private void CloseDoor()
    {
        float closedAngle = 0f;
        JointSpring hingeSpring = ovenDoorHinge.spring;
        hingeSpring.targetPosition = closedAngle;
        ovenDoorHinge.spring = hingeSpring;
    }

}
