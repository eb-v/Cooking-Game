using UnityEngine;

[CreateAssetMenu(fileName = "AS_Idle", menuName = "Scriptable Objects/States/Kitchen/Assembly/Idle")]
public class AssemblyIdle : AssemblyState
{
    private AssemblyStation assemblyStation;

    public override void Enter()
    {
        base.Enter();
        GenericEvent<AssemblyStationColliderEntered>.GetEvent("AssemblyStation").AddListener(OnItemReceived);
    }

    public override void Exit()
    {
        base.Exit();
        GenericEvent<AssemblyStationColliderEntered>.GetEvent("AssemblyStation").RemoveListener(OnItemReceived);
    }

    public override void FixedUpdateLogic()
    {
        base.FixedUpdateLogic();
    }

    public override void Initialize(GameObject gameObject, StateMachine<AssemblyState> stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
        assemblyStation = gameObject.GetComponent<AssemblyStation>();
    }

    public override void InteractLogic(GameObject player)
    {
        base.InteractLogic(player);
        MenuItem selectedMenuItem = assemblyStation.SelectedMenuItem;
        
        if (CanAssembleSelectedItem(selectedMenuItem))
        {
            UseIngredientsToMakeItem(selectedMenuItem);
            assemblyStation.ChangeState(assemblyStation.AssembleStateInstance);
        }
        else
        {
            Debug.Log("Cannot assemble Item");
        }
        
    }

    public override void AltInteractLogic(GameObject player)
    {
        base.AltInteractLogic(player);
        ChangeSelectedMenuItem();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    private void OnItemReceived(GameObject item)
    {
        if (IsItemValid(item))
        {
            // Add Item to Storage
            IngredientScript ingredientScript = item.GetComponent<IngredientScript>();
            AddToStorage(ingredientScript.Ingredient);
            Destroy(item);
        }
        else
        {
            Debug.Log("Invalid Item for Assembly Station");
        }
    }

    private bool IsItemValid(GameObject item)
    {
        IngredientScript ingredient = item.GetComponent<IngredientScript>();
        if (ingredient == null)
            return false;

        if (!assemblyStation.ingredientStorage.ContainsKey(ingredient.Ingredient))
            return false;

        return true;
    }

    private void AddToStorage(Ingredient ingredient)
    {
        assemblyStation.ingredientStorage[ingredient]++;
        int amountStored = assemblyStation.ingredientStorage[ingredient];
        GenericEvent<IngredientStorageAmountChanged>.GetEvent("AssemblyStation").Invoke(ingredient, amountStored);
    }

    private void ChangeSelectedMenuItem()
    {
        assemblyStation.selectedMenuItemIndex = (assemblyStation.selectedMenuItemIndex + 1) % assemblyStation.availableMenuItems.Count;
        assemblyStation.selectedMenuItem = assemblyStation.availableMenuItems[assemblyStation.selectedMenuItemIndex];
    }

    private bool CanAssembleSelectedItem(MenuItem selectedMenuItem)
    {
        foreach (Ingredient ingredient in selectedMenuItem.IngredientsNeeded)
        {
            if (assemblyStation.ingredientStorage.ContainsKey(ingredient))
            {
                if (assemblyStation.ingredientStorage[ingredient] > 0)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private void UseIngredientsToMakeItem(MenuItem menuItem)
    {
        foreach (Ingredient ingredient in menuItem.IngredientsNeeded)
        {
            assemblyStation.ingredientStorage[ingredient]--;
            int amountStored = assemblyStation.ingredientStorage[ingredient];
            GenericEvent<IngredientStorageAmountChanged>.GetEvent("AssemblyStation").Invoke(ingredient, amountStored);
        }
    }

}
