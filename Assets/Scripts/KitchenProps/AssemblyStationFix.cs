using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssemblyStationFix : MonoBehaviour
{
    [SerializeField] private DeliveryManager deliveryManager;
    [SerializeField] private GameObject wrongFinishedItem;
    [SerializeField] private GameObject _boxLid;
    [SerializeField] private float _lowerLimitY = -2.07f;
    [SerializeField] private float _raiseLimitY = 0.88f;
    [SerializeField] private float _assemblyTime = 5.0f;
    [SerializeField] private Transform _productSpawnPoint;
    [SerializeField] private GameObject _finalProductPrefab;

    [SerializeField] private List<RecipeSO> _availableRecipes;
    [SerializeField] private RecipeSO _selectedRecipe;
    [SerializeField] private Image _selectedRecipeUIImage;
    

    private int _currentRecipeIndex = 0;

    public List<GameObject> ingredientsInAssemblyArea;





    public float verticalSpeed = 0.02f;

    public float _assemblyTimer = 0.0f;
    private bool _isOn;

    private void Awake()
    {
        ingredientsInAssemblyArea = new List<GameObject>();
        GenericEvent<IngredientEnteredAssemblyArea>.GetEvent(gameObject.name).AddListener(AddIngredientToSet);
        GenericEvent<IngredientExitedAssemblyArea>.GetEvent(gameObject.name).AddListener(RemoveIngredientFromSet);
        GenericEvent<Interact>.GetEvent(gameObject.name).AddListener(ActivateAssembler);
        //GenericEvent<AlternateInteractInput>.GetEvent(gameObject.name).AddListener(ChangeSelectedRecipe);
    }

    private void Start()
    {
        _selectedRecipe = _availableRecipes[_currentRecipeIndex];
        _finalProductPrefab = _selectedRecipe.finalProductPrefab;
        _selectedRecipeUIImage.sprite = _selectedRecipe.finalProductImageUI;
        //TurnOn();
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

    private void ActivateAssembler()
    {
        if (!_isOn)
        {
            TurnOn();
        }
        else
        {
            Debug.Log("Assembly Station is already on.");
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
        Debug.Log(boxLidPos.y);

    }

    private bool FinishedLowering()
    {
        //if (_boxLid.transform.position.y < _lowerLimitY)
        //{
        //    Vector3 boxLidPos = _boxLid.transform.position;
        //    boxLidPos.y = _lowerLimitY;
        //    _boxLid.transform.position = boxLidPos;
        //}
        Debug.Log(_boxLid.transform.position.y <= _lowerLimitY);
        return _boxLid.transform.position.y <= _lowerLimitY;
    }

    private void AssembleProduct()
    {
            if (_assemblyTimer < _assemblyTime)
        {
            _assemblyTimer += Time.fixedDeltaTime;
            return;
        }

        Vector3 spawnPos = _productSpawnPoint.position;

        //temp AssembledItemObject from ingredientsInAssemblyArea
        GameObject tempAssembledGO = new GameObject("TempAssembled");
        AssembledItemObject tempAssembled = tempAssembledGO.AddComponent<AssembledItemObject>();
        foreach (GameObject ingredient in ingredientsInAssemblyArea)
        {
            if (ingredient != null)
                tempAssembled.AddIngredient(ingredient);
        }

        //check for matched recipe
        RecipeSO matchedRecipe = deliveryManager.GetMatchingRecipeFromAll(tempAssembled);
        GameObject finalPrefab = (matchedRecipe != null) ? matchedRecipe.finalProductPrefab : wrongFinishedItem;

        //spawn
        GameObject newProduct = Instantiate(finalPrefab, spawnPos, Quaternion.identity);
        newProduct.tag = "AssembledItem";

        //ensure has AssembledItemObject
        AssembledItemObject newItemComponent = newProduct.GetComponent<AssembledItemObject>();
        if (newItemComponent == null)
            newItemComponent = newProduct.AddComponent<AssembledItemObject>();

        //copy ingredient data
        foreach (GameObject ingredient in ingredientsInAssemblyArea)
        {
            if (ingredient != null)
            {
                newItemComponent.AddIngredient(ingredient);
                ingredient.SetActive(false);
                Rigidbody rb = ingredient.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;
                Collider col = ingredient.GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }

        ingredientsInAssemblyArea.Clear();
        Destroy(tempAssembledGO);

        _assemblyTimer = 0.0f;
        TurnOff();

        Debug.Log($"Spawned {(matchedRecipe != null ? "final product" : "custom assembled item")} at {spawnPos}");
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
        //if (_boxLid.transform.position.y > _raiseLimitY)
        //{
        //    Vector3 boxLidPos = _boxLid.transform.position;
        //    boxLidPos.y = _raiseLimitY;
        //    _boxLid.transform.position = boxLidPos;
        //}

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
/*
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
    }*/

/*
    private bool CheckIngredientsInAssemblyArea(GameObject requiredIngredientPrefab)
    {
        foreach (GameObject ingredient in ingredientsInAssemblyArea)
        {
            // take out (clone) from the name of the ingredient
            string ingredientName = ingredient.GetComponent<Ingredient>().IngredientName.Replace("(Clone)", "").Trim();
            if (ingredientName == requiredIngredientPrefab.name)
            {
                return true;
            }
        }
        return false;
    }*/

    // only use after confirming all ingredients are present
/*
    private void UseIngredients(RecipeSO currentRecipe)
    {

        foreach (CuttingRecipeSO cuttingRecipe in currentRecipe.CuttingRecipeSOList)
        {
            GameObject requiredIngredientPrefab = cuttingRecipe.output;

            foreach (GameObject ingredient in ingredientsInAssemblyArea)
            {
                string ingredientName = ingredient.GetComponent<Ingredient>().IngredientName.Replace("(Clone)", "").Trim();
                if (ingredientName == requiredIngredientPrefab.name)
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
                string ingredientName = ingredient.GetComponent<Ingredient>().IngredientName.Replace("(Clone)", "").Trim();
                if (ingredientName == requiredIngredientPrefab.name)
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
*/

/*
    private void ChangeSelectedRecipe(GameObject player)
    {
        if (!_isOn)
        {
            _currentRecipeIndex++;
            if (_currentRecipeIndex >= _availableRecipes.Count)
            {
                _currentRecipeIndex = 0;
            }
            _selectedRecipe = _availableRecipes[_currentRecipeIndex];
            _finalProductPrefab = _selectedRecipe.finalProductPrefab;
            _selectedRecipeUIImage.sprite = _selectedRecipe.finalProductImageUI;
        }
        else
        {
            Debug.Log("Cannot change recipe while assembly station is on.");
        }
    }
*/

}
