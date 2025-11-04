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
    [Header("Movement")]
    [SerializeField] private Transform dispenserTransform;
    [SerializeField] private Transform raycastTransform;
    [SerializeField] private float lowerDistance = 2f;
    [SerializeField] private float raiseDistance = 2f;
    [SerializeField] private float speed = 1f;
    private DispenserState currentState = DispenserState.Idle;

    [Header("Detection")]
    [SerializeField] private float interactionRange = 0.5f;
    private int layerMask;

    [Header("Dispense Timing")]
    [SerializeField] private float dispenseDuration = 1.0f;
    private float dispenseTimer = 0f;

    [Header("Ingredient")]
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private IngredientType dispenserIngredientType; // what this dispenser is for

    [Header("Supply Settings")]
    [SerializeField] private int maxUses = 10;
    [SerializeField] private int startingUses = 10;
    [SerializeField] private int currentUses;
    [SerializeField] private bool isEmpty = false;
    public bool IsEmpty => isEmpty;

    private void Awake()
    {
        layerMask = (1 << LayerMask.NameToLayer("Ingredients"));

        // cache the "up" position as the raise distance
        raiseDistance = dispenserTransform.localPosition.y;

        // init supply
        currentUses = Mathf.Clamp(startingUses, 0, maxUses);
        isEmpty = currentUses <= 0;

        // auto-detect ingredient type from prefab if possible
        if (ingredientPrefab != null)
        {
            IngredientTag tag = ingredientPrefab.GetComponent<IngredientTag>();
            if (tag != null)
            {
                dispenserIngredientType = tag.type;
            }
        }
    }

    private void Start()
    {
        GenericEvent<DispenserButtonPressedEvent>
            .GetEvent(gameObject.name)
            .AddListener(ActivateDispenser);
    }

    private void ActivateDispenser()
    {
        // only run if idle AND has supply
        if (currentState == DispenserState.Idle && !isEmpty)
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
        // nothing for now
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
        Ray ray = new Ray(raycastTransform.position, Vector3.down);
        bool dispensedIngredient = false;

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, layerMask))
        {
            PizzaDoughBase pizzaBase = hit.collider.gameObject.GetComponent<PizzaDoughBase>();
            if (pizzaBase != null)
            {
                // only try to dispense if we still have supply
                if (!isEmpty && !pizzaBase.CheckForIngredient(ingredientPrefab))
                {
                    pizzaBase.AddIngredient(ingredientPrefab);
                    dispensedIngredient = true;

                    // use one charge
                    currentUses = Mathf.Max(0, currentUses - 1);
                    if (currentUses <= 0)
                    {
                        isEmpty = true;
                        // hook "!" indicator off this flag
                    }
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

    // called by the supply box when the correct ingredient is deposited
    public void Refill(int amount)
    {
        currentUses = Mathf.Clamp(currentUses + amount, 0, maxUses);
        if (currentUses > 0)
        {
            isEmpty = false;
        }
    }

    // for the supply box to know what type this dispenser expects
    public IngredientType GetIngredientType()
    {
        return dispenserIngredientType;
    }
}
