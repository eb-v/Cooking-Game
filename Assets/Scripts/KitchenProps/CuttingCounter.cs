using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;

public class CuttingCounter : MonoBehaviour, IInteractable, IAltInteractable
{
    [Header("Snap Points")]
    [SerializeField] private Transform _playerSnapPoint;
    [SerializeField] private Transform _objectSnapPoint;


    private GameObject _currentObject;
    private GameObject _currentPlayer;

    private CuttingRecipe _currentRecipe;

    private int cuttingProgress = 0;

    private bool hasObject => _currentObject != null;
    private bool notBeingUsed => _currentPlayer == null;

    public void OnInteract(GameObject player)
    {
        GrabScript gs = player.GetComponent<GrabScript>();

        // ingredient placement logic
        if (gs.isGrabbing)
        {
            GameObject grabbedObject = gs.grabbedObject.GetGameObject();
            if (grabbedObject.GetComponent<IngredientScript>() == null)
            {
                Debug.Log("Object is not an ingredient, cannot place on cutting counter.");
                return;
            }
            else
            {

            }

            if (hasObject)
            {
                Debug.Log("Counter Already Occupied");
                return;
            }
            else
            {
                IngredientScript ingredient = grabbedObject.GetComponent<IngredientScript>();
                if (IsCuttable(ingredient))
                {
                    PlaceObjectOntoCounter(gs.grabbedObject);
                    _currentRecipe = ingredient.recipes.Find(recipe => recipe is CuttingRecipe) as CuttingRecipe;

                    gs.grabbedObject = null;
                }
                else
                {
                    Debug.Log("Ingredient is not cuttable.");
                }
            }
        }
        // start cutting logic
        else
        {
            if (hasObject)
            {
                if (notBeingUsed)
                {
                    RagdollController rc = player.GetComponent<RagdollController>();
                    Transform rootTransform = rc.GetPelvis().gameObject.transform;

                    rootTransform.position = _playerSnapPoint.position;

                    Player playerScript = player.GetComponent<Player>();
                    Vector3 newForwardDirection = (_objectSnapPoint.position - _playerSnapPoint.position).normalized;
                    newForwardDirection.y = 0;
                    rc.SetForwardDirection(newForwardDirection);
                    playerScript.ChangeState(playerScript._cuttingStateInstance);
                    rootTransform.rotation = _playerSnapPoint.rotation;

                    _currentPlayer = player;
                    GenericEvent<OnRightTriggerInput>.GetEvent(player.name).AddListener(CutIngredient);
                }

            }
            else
            {
                Debug.Log("No Object On Counter To Cut");
            }
        }

    }

    public void OnAltInteract(GameObject player)
    {
        GrabScript gs = player.GetComponent<GrabScript>();

        if (gs.isGrabbing)
            return;

        if (_currentObject == null)
        {
            Debug.Log("no object on cutting counter to remove");
            return;
        }

        GameObject objToGrab = RemoveObjectFromCounter();
        IGrabable grabable = objToGrab.transform.root.GetComponent<IGrabable>();
        if (grabable == null)
        {
            Debug.LogError("Grabable not found on object when trying to remove it from counter");
            return;
        }

        grabable.GrabObject(player);
    }

    private void PlaceObjectOntoCounter(IGrabable grabbedObj)
    {
        _currentObject = grabbedObj.GetGameObject();
        grabbedObj.ReleaseObject(grabbedObj.currentPlayer);

        Transform physicsObj = grabbedObj.GetGameObject().GetComponent<PhysicsTransform>().physicsTransform;

        if (physicsObj == null)
        {
            Debug.LogError("The grabbed object does not have a PhysicsTransform component.");
            return;
        }

        physicsObj.transform.position = _objectSnapPoint.position;
        physicsObj.transform.rotation = Quaternion.identity;

        Rigidbody rb = physicsObj.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private GameObject RemoveObjectFromCounter()
    {

        GameObject physicsObj = _currentObject.GetComponent<PhysicsTransform>().physicsTransform.gameObject;

        if (physicsObj == null)
        {
            Debug.LogError("Physics obj not found when removing object from counter");
            return null;
        }

        Rigidbody rb = physicsObj.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        _currentObject = null;

        return physicsObj;
    }

    private void CutIngredient()
    {
        cuttingProgress++;
        if (cuttingProgress >= 3)
        {
            Debug.Log("Ingredient fully cut!");
            // Here you can implement logic to change the ingredient state to "cut"
            cuttingProgress = 0; // Reset for next ingredient

            IngredientScript ingredientScript = _currentObject.GetComponent<IngredientScript>();
            foreach (Ingredient output in _currentRecipe.output)
            {
                GameObject ingredientObj = Instantiate(output.Prefab, _objectSnapPoint.position, Quaternion.identity);
                Rigidbody rb = ingredientObj.GetComponentInChildren<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = (rb.transform.position - _playerSnapPoint.position).normalized;

                    rb.AddForce(direction * 10f, ForceMode.Impulse);
                }
            }



            Destroy(_currentObject);
            _currentObject = null;
            _currentRecipe = null;

            Player playerScript = _currentPlayer.GetComponent<Player>();
            playerScript.ChangeState(playerScript._defaultStateInstance);
            GenericEvent<OnRightTriggerInput>.GetEvent(_currentPlayer.name).RemoveListener(CutIngredient);

        }
        else
        {
            Debug.Log("Cutting progress: " + cuttingProgress);
        }
    }

    private bool IsCuttable(IngredientScript ingredient)
    {
        foreach (var recipe in ingredient.recipes)
        {
            if (recipe is CuttingRecipe)
            {
                return true;
            }
            else
            {
                continue;
            }
        }
        return false;
    }


}
