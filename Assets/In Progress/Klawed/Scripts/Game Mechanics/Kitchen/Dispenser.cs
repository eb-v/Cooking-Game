using UnityEngine;

public enum DispenserState
{
    Idle,
    Lowering,
    Dispensing,
    Raising
}

public class Dispenser : MonoBehaviour
{
    [SerializeField] private Transform dispenserTransform;
    [SerializeField] private Transform raycastTransform;
    [SerializeField] private float lowerDistance = 2f;
    [SerializeField] private float raiseDistance = 2f;
    [SerializeField] private float speed = 1f;
    private DispenserState currentState = DispenserState.Idle;
    [SerializeField] private float interactionRange = 0.5f;
    private int layerMask;
    [SerializeField] private float dispenseDuration = 1.0f;
    private float dispenseTimer = 0f;

    [SerializeField] private GameObject ingredientPrefab;
    // Name of the container object on the Pizza base that will contain the dispensed ingredient gameObject

    private void Awake()
    {
        layerMask = (1 << LayerMask.NameToLayer("Ingredients"));
        raiseDistance = dispenserTransform.localPosition.y;
    }

    private void Start()
    {
        GenericEvent<DispenserButtonPressedEvent>.GetEvent(gameObject.name).AddListener(ActivateDispenser);
    }

    private void ActivateDispenser()
    {
        if (currentState == DispenserState.Idle)
        {
            ChangeState(DispenserState.Lowering);
        }
    }

    private void Update()
    {
        RunDispenserLogic();
    }

    private void RunDispenserLogic()
    {
        switch (currentState)
        {
            case DispenserState.Idle:
                RunIdleLogic();
                break;
            case DispenserState.Lowering:
                RunLoweringLogic();
                break;
            case DispenserState.Dispensing:
                RunDispensingLogic();
                break;
            case DispenserState.Raising:
                RunRaisingLogic();
                break;
        }

    }


    private void RunIdleLogic()
    {

    }

    private void RunLoweringLogic()
    {
        Vector3 currentPos = dispenserTransform.localPosition;
        currentPos.y -= speed * Time.deltaTime;
        dispenserTransform.localPosition = currentPos;
        if (dispenserTransform.localPosition.y <= lowerDistance)
        {
            ChangeState(DispenserState.Dispensing);
        }
    }

    private void RunDispensingLogic()
    {
        // Dispensing logic here (e.g., instantiate item)
        Ray ray = new Ray(raycastTransform.position, Vector3.down);
        bool dispensedIngredient = false;

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, layerMask))
        {
            // check if the hit object is a pizza dough base
            PizzaDoughBase pizzaBase = hit.collider.gameObject.GetComponent<PizzaDoughBase>();
            if (pizzaBase != null)
            {
                // does the pizza base already have an ingredient of this type?
                // check is done through the prefab rather than an instance of the ingredient
                if (!pizzaBase.CheckForIngredient(ingredientPrefab))
                {
                    // pass ingredient to PizzaDoughBase to add and create instance of ingredient
                    pizzaBase.AddIngredient(ingredientPrefab);
                    dispensedIngredient = true;
                }

                Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.green);
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);
        }


        dispenseTimer += Time.deltaTime;
        if (dispenseTimer >= dispenseDuration || dispensedIngredient)
        {
            dispenseTimer = 0f;
            ChangeState(DispenserState.Raising);
        }
    }

    private void RunRaisingLogic()
    {
        Vector3 currentPos = dispenserTransform.localPosition;
        currentPos.y += speed * Time.deltaTime;
        dispenserTransform.localPosition = currentPos;
        if (dispenserTransform.localPosition.y >= raiseDistance)
        {
            ChangeState(DispenserState.Idle);
        }
    }

    private void ChangeState(DispenserState newState)
    {
        currentState = newState;
    }

    private void DispenseIngredient()
    {
        
    }


}