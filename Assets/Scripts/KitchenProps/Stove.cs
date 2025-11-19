using UnityEditor.Rendering;
using UnityEngine;

public class Stove : MonoBehaviour, IInteractable, IAltInteractable
{
    [SerializeField] private Transform objectSnapPoint;

    private CookingRecipe _currentRecipe;


    private GameObject _currentObject;

    private bool hasObject => _currentObject != null;

    private StateMachine _stateMachine;


    private void Awake()
    {
        _stateMachine = new StateMachine();


    }

    public void OnAltInteract(GameObject player)
    {
        if (!hasObject)
            return;
        IGrabable grabable = _currentObject.GetComponent<IGrabable>();
        RemoveObjectFromStove(grabable);

    }

    public void OnInteract(GameObject player)
    {
        if (hasObject)
            return;

        GrabScript gs = player.GetComponent<GrabScript>();
        if (gs.isGrabbing)
        {
            GameObject grabbedObject = gs.grabbedObject.GetGameObject();

            if (!IsItemCompatible(grabbedObject))
            {
                Debug.Log("Object is not compatible with the stove.");
                return;
            }

            PlaceObjectOntoStove(gs.grabbedObject);
        }
    }

    private void PlaceObjectOntoStove(IGrabable grabbedObj)
    {
        Debug.Log("Placing object onto stove.");
    }

    private void RemoveObjectFromStove(IGrabable grabbedObj)
    {
        Debug.Log("Removing object from stove.");
    }

    private void Update()
    {
        _stateMachine.RunUpdateLogic();
    }

    private void FixedUpdate()
    {
        _stateMachine.RunFixedUpdateLogic();
    }

    public void ChangeState(BaseStateSO newState)
    {
        _stateMachine.ChangeState(newState);
    }


    private bool IsItemCompatible(GameObject objectToCheck)
    {
        if (objectToCheck.GetComponent<IngredientScript>() == null)
            return false;

        IngredientScript ingredient = objectToCheck.GetComponent<IngredientScript>();

        CookingRecipe cookingRecipe = ingredient.recipes.Find(recipe => recipe is CookingRecipe) as CookingRecipe;

        if (cookingRecipe == null)
            return false;

        _currentRecipe = cookingRecipe;

        return true;
    }




}
