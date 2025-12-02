using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;

public class CuttingCounter : MonoBehaviour
{
    private enum CuttingState
    {
        Idle,
        Cutting,
    }

    [Header("Settings")]
    [SerializeField] private float _cutsNeeded = 3;
    [SerializeField] private float _ingredientForceMultiplier = 10f;

    [Header("Designated Animation Controller")]
    [SerializeField] private RuntimeAnimatorController _cuttingAnimator;

    [Header("Snap Points")]
    [SerializeField] private Transform _playerSnapPoint;
    [SerializeField] private Transform _objectSnapPoint;

    [Header("Cutting VFX")]
    [SerializeField] private ParticleSystem _cuttingVFX;

    [Header("Debug")]
    [ReadOnly]
    [SerializeField] private GameObject _currentObject;
    [ReadOnly]
    [SerializeField] private Player _currentPlayer;
    [ReadOnly]
    [SerializeField] private CuttingState _currentState = CuttingState.Idle;

    private CuttingRecipe _currentRecipe;

    private int cuttingProgress = 0;

    private bool hasObject => _currentObject != null;
    private bool notBeingUsed => _currentPlayer == null;

    private void OnEnable()
    {
        GenericEvent<OnObjectIgnited>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnCuttingBoardIgnited);
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnAltInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnObjectIgnited>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnCuttingBoardIgnited);
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnAltInteract);
    }

    private void OnInteract(GameObject player)
    {
        GrabScript gs = player.GetComponent<GrabScript>();
        if (gs == null) return;

        // ingredient placement logic
        if (gs.IsGrabbing)
        {
            GameObject grabbedObject = gs.grabbedObject.gameObject;
            if (grabbedObject.GetComponent<IngredientScript>() == null)
            {
                Debug.Log("Object is not an ingredient, cannot place on cutting counter.");
                return;
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
                    EnterCutState(player);
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
        if (!notBeingUsed)
        {
            if (_currentPlayer == player.GetComponent<Player>())
            {
                _currentPlayer.ChangeState(_currentPlayer._defaultStateInstance);
                UnAssignPlayer();
            }
        }
        else
        {
            GrabScript gs = player.GetComponent<GrabScript>();

            if (gs.IsGrabbing)
                return;

            if (_currentObject == null)
            {
                Debug.Log("no object on cutting counter to remove");
                return;
            }

            GameObject removedObj = RemoveObjectFromCounter();
            Grabable grabable = removedObj.GetComponent<Grabable>();
            grabable.Grab(player);
        }
    }

    private void PlaceObjectOntoCounter(Grabable grabComponent)
    {
        _currentObject = grabComponent.gameObject;
        grabComponent.Release();
        grabComponent.grabCollider.enabled = false;

        _currentObject.transform.position = _objectSnapPoint.position;
        _currentObject.transform.rotation = Quaternion.identity;

        Rigidbody rb = _currentObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private GameObject RemoveObjectFromCounter()
    {
        Rigidbody rb = _currentObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        Grabable grabable = _currentObject.GetComponent<Grabable>();
        grabable.grabCollider.enabled = true;

        GameObject removedObj = _currentObject;
        _currentObject = null;
        return removedObj;
    }

    private void CutIngredient()
    {
        cuttingProgress++;

        AudioManager.Instance?.PlaySFX("Cut");

        if (_cuttingVFX != null)
        {
            _cuttingVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _cuttingVFX.Play();
        }

        RagdollController rc = _currentPlayer.GetComponent<RagdollController>();

        if (cuttingProgress >= _cutsNeeded)
        {
            OnFinishedCut();
        }
        else
        {
            Debug.Log("Cutting progress: " + cuttingProgress);
        }
    }

    private void OnFinishedCut()
    {
        cuttingProgress = 0;

        IngredientScript ingredientScript = _currentObject.GetComponent<IngredientScript>();
        foreach (Ingredient output in _currentRecipe.output)
        {
            GameObject ingredientObj = Instantiate(output.Prefab, _objectSnapPoint.position, Quaternion.identity);
            Rigidbody rb = ingredientObj.GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                Vector3 forceDirection = (_playerSnapPoint.position - _objectSnapPoint.position).normalized;
                rb.AddForce(forceDirection * _ingredientForceMultiplier, ForceMode.Impulse);
                rb.AddForce(Vector3.up * (_ingredientForceMultiplier), ForceMode.Impulse);
            }
        }

        Destroy(_currentObject);
        _currentObject = null;
        _currentRecipe = null;
        ExitCutState();
    }

    private bool IsCuttable(IngredientScript ingredient)
    {
        foreach (var recipe in ingredient.recipes)
        {
            if (recipe is CuttingRecipe)
            {
                return true;
            }
        }
        return false;
    }

    private void EnterCutState(GameObject player)
    {
        PlayerInteraction pi = player.GetComponent<PlayerInteraction>();
        pi.enabled = false;
        Player playerScript = player.GetComponent<Player>();
        _currentPlayer = playerScript;
        RagdollController rc = _currentPlayer.GetComponent<RagdollController>();
        Transform rootTransform = rc.GetPelvis().gameObject.transform;
        rootTransform.position = _playerSnapPoint.position;
        rootTransform.rotation = _playerSnapPoint.rotation;
        playerScript.SwitchToAnimationMode(_cuttingAnimator);

        GenericEvent<OnPerformStationAction>.GetEvent(player.name).AddListener(CutIngredient);
        GenericEvent<OnAlternateInteractInput>.GetEvent(player.name).AddListener(ExitCutState);
        ChangeState(CuttingState.Cutting);
    }

    private void ExitCutState()
    {
        PlayerInteraction pi = _currentPlayer.GetComponent<PlayerInteraction>();
        pi.enabled = true;
        _currentPlayer.ChangeState(_currentPlayer._defaultStateInstance);


        GenericEvent<OnPerformStationAction>.GetEvent(_currentPlayer.name).RemoveListener(CutIngredient);
        GenericEvent<OnAlternateInteractInput>.GetEvent(_currentPlayer.name).RemoveListener(ExitCutState);
        UnAssignPlayer();
        ChangeState(CuttingState.Idle);
    }

    private void UnAssignPlayer()
    {
        _currentPlayer = null;
    }

    private void ChangeState(CuttingState newState)
    {
        _currentState = newState;
    }

    private void OnCuttingBoardIgnited()
    {
        if (hasObject)
        {
            Destroy(_currentObject);
            _currentObject = null;
            _currentRecipe = null;
            cuttingProgress = 0;
        }

        if (_currentState == CuttingState.Cutting)
        {
            ExitCutState();
        }
    }
}
