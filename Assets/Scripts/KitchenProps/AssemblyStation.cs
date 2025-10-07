using System.Collections.Generic;
using UnityEngine;

public class AssemblyStation : MonoBehaviour
{
    [SerializeField] private GameObject _boxLid;
    [SerializeField] private float _lowerLimitY = -2.07f;
    [SerializeField] private float _raiseLimitY = 0.88f;
    [SerializeField] private float _assemblyTime = 5.0f;
    [SerializeField] private Transform _productSpawnPoint;
    [SerializeField] private GameObject _finalProductPrefab;

    [SerializeField] private List<RecipeSO> _availableRecipes;
    [SerializeField] private RecipeSO _selectedRecipe;

    private int _currentRecipeIndex = 0;

    private readonly HashSet<GameObject> ingredientsInAssemblyArea = new HashSet<GameObject>();





    public float verticalSpeed = 0.02f;

    public float _assemblyTimer = 0.0f;
    private bool _isOn;

    private void Awake()
    {
        GenericEvent<IngredientEnteredAssemblyArea>.GetEvent(gameObject.name).AddListener(AddIngredientToSet);
        GenericEvent<IngredientExitedAssemblyArea>.GetEvent(gameObject.name).AddListener(RemoveIngredientFromSet);
        // GenericEvent<AlternateInteractInput>.GetEvent(gameObject.name).AddListener(ChangeSelectedRecipe);
    }

    private void Start()
    {
        TurnOn();
    }

    private void FixedUpdate()
    {
        if (_isOn)
        {
            if (!FinishedLowering())
            {
                LowerBoxLid();
            }
            else
            {
                AssembleProduct();
            }
        }
        else
        {
            if (!FinishedRaising())
            {
                RaiseBoxLid();
            }
        }
    }


    private void TurnOn()
    {
        _isOn = true;
    }


    private void TurnOff()
    {
        _isOn = false;
    }


    private void LowerBoxLid()
    {
        Transform boxlidTransform = _boxLid.transform;
        Vector3 boxLidPos = boxlidTransform.position;

        boxLidPos.y -= verticalSpeed;

        boxlidTransform.position = boxLidPos;

    }

    private bool FinishedLowering()
    {
        if (_boxLid.transform.position.y < _lowerLimitY)
        {
            Vector3 boxLidPos = _boxLid.transform.position;
            boxLidPos.y = _lowerLimitY;
            _boxLid.transform.position = boxLidPos;
        }

        return _boxLid.transform.position.y <= _lowerLimitY;
    }

    private void AssembleProduct()
    {
        // Logic to assemble the product goes here
        if (_assemblyTimer < _assemblyTime)
        {
            _assemblyTimer += Time.fixedDeltaTime;
        }
        else
        {
            // check if all required ingredients are present
            if (CheckForRecipeRequirements(_selectedRecipe))
            {
                // All ingredients are present, proceed with assembly

                // Remove used ingredients from the assembly area
                UseIngredients(_selectedRecipe);

                // spawn the final product at the spawn point

            }



            Debug.Log("Product Assembled!");
            ObjectPoolManager.SpawnObject(_finalProductPrefab, _productSpawnPoint.position, Quaternion.identity);


            _assemblyTimer = 0.0f;
            TurnOff(); // Turn off the station after assembling

        }
    }

    private void RaiseBoxLid()
    {
        Transform boxlidTransform = _boxLid.transform;
        Vector3 boxLidPos = boxlidTransform.position;
        boxLidPos.y += verticalSpeed;
        boxlidTransform.position = boxLidPos;
    }

    private bool FinishedRaising()
    {
        if (_boxLid.transform.position.y > _raiseLimitY)
        {
            Vector3 boxLidPos = _boxLid.transform.position;
            boxLidPos.y = _raiseLimitY;
            _boxLid.transform.position = boxLidPos;
        }

        return _boxLid.transform.position.y >= _raiseLimitY;
    }

    private void AddIngredientToSet(GameObject ingredient)
    {
        ingredientsInAssemblyArea.Add(ingredient);
    }

    private void RemoveIngredientFromSet(GameObject ingredient)
    {
        ingredientsInAssemblyArea.Remove(ingredient);
    }

    // based on the selected recipe, check if all required ingredients are present in the assembly area
    private bool CheckForRecipeRequirements(RecipeSO recipe)
    {
        foreach (CuttingRecipeSO cuttingRecipe in recipe.CuttingRecipeSOList)
        {
            GameObject requiredIngredientPrefab = cuttingRecipe.output;
            if (!CheckIngredientsInAssemblyArea(requiredIngredientPrefab))
            {
                // if ingredient for this recipe is not found, return false
                return false;
            }
            else
            {
                // if ingredient for this recipe is found, check next recipe
                continue;
            }
        }

        foreach (CookingRecipeSO cookingRecipe in recipe.CookingRecipeSOList)
        {
            GameObject requiredIngredientPrefab = cookingRecipe.output;
            if (!CheckIngredientsInAssemblyArea(requiredIngredientPrefab))
            {
                // if ingredient for this recipe is not found, return false
                return false;
            }
            else
            {
                // if ingredient for this recipe is found, check next recipe
                continue;
            }
        }
        // all ingredients for this recipe are present
        return true;
    }

    private bool CheckIngredientsInAssemblyArea(GameObject requiredIngredientPrefab)
    {
        foreach (GameObject ingredient in ingredientsInAssemblyArea)
        {
            if (ingredient.GetComponent<Ingredient>().IngredientPrefab == requiredIngredientPrefab)
            {
                return true;
            }
        }

        return false;

    }
    // only use after confirming all ingredients are present
    private void UseIngredients(RecipeSO currentRecipe)
    {

        foreach (CuttingRecipeSO cuttingRecipe in currentRecipe.CuttingRecipeSOList)
        {
            GameObject requiredIngredientPrefab = cuttingRecipe.output;

            foreach (GameObject ingredient in ingredientsInAssemblyArea)
            {
                if (ingredient.GetComponent<Ingredient>().IngredientPrefab == requiredIngredientPrefab)
                {
                    ingredientsInAssemblyArea.Remove(ingredient);
                    ObjectPoolManager.ReturnObjectToPool(ingredient);
                    break;
                }
            }
        }

        foreach (CookingRecipeSO cookingRecipe in currentRecipe.CookingRecipeSOList)
        {
            GameObject requiredIngredientPrefab = cookingRecipe.output;

            foreach (GameObject ingredient in ingredientsInAssemblyArea)
            {
                if (ingredient.GetComponent<Ingredient>().IngredientPrefab == requiredIngredientPrefab)
                {
                    ingredientsInAssemblyArea.Remove(ingredient);
                    ObjectPoolManager.ReturnObjectToPool(ingredient);
                    break;
                }
            }

        }

    }

    private void CheckCuttingRecipeList(RecipeSO recipe)
    {
        foreach (CuttingRecipeSO cuttingRecipe in recipe.CuttingRecipeSOList)
        {
            GameObject requiredIngredientPrefab = cuttingRecipe.output;

            if (CheckIngredientsInAssemblyArea(requiredIngredientPrefab))
            {
                // if ingredient for this recipe is found, check next recipe
                continue;
            }

        }
    }

    private void ChangeSelectedRecipe()
    {
        _currentRecipeIndex++;
        if (_currentRecipeIndex >= _availableRecipes.Count)
        {
            _currentRecipeIndex = 0;
        }
        _selectedRecipe = _availableRecipes[_currentRecipeIndex];
        _finalProductPrefab = _selectedRecipe.finalProductPrefab;
    }

    private void SpawnFinalProduct()
    {
        ObjectPoolManager.SpawnObject(_finalProductPrefab, _productSpawnPoint.position, Quaternion.identity);
    }


}
