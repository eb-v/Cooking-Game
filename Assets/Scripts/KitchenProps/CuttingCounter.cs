using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;

public class CuttingCounter : MonoBehaviour, IInteractable, IAltInteractable
{
    private enum CuttingState
    {
        Idle,
        Cutting,
    }

    [Header("Settings")]
    [SerializeField] private float _cutsNeeded = 3;
    [SerializeField] private float _ingredientForceMultiplier = 10f;

    [Header("Player Pose Data")]
    [SerializeField] private PoseData _preChopPose;
    [SerializeField] private PoseData _postChopPose;

    [Header("Snap Points")]
    [SerializeField] private Transform _playerSnapPoint;
    [SerializeField] private Transform _objectSnapPoint;

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

    public void OnInteract(GameObject player)
    {
        GrabScript gs = player.GetComponent<GrabScript>();

        // ingredient placement logic
        if (gs.IsGrabbing)
        {
            GameObject grabbedObject = gs.grabbedObject.GetGameObject();
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
                // if player is holding an ingredient, check if it's cuttable, if it is, place it on the counter and set current recipe
                IngredientScript ingredient = grabbedObject.GetComponent<IngredientScript>();
                if (IsCuttable(ingredient))
                {
                    gs.grabbedObject.grabCollider.enabled = false;
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
            // if player is not holding anything, and counter has an object, snap player to counter and start cutting
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
            IGrabable grabable = removedObj.GetComponent<IGrabable>();
            grabable.GrabObject(player);
        }
    }

    private void PlaceObjectOntoCounter(IGrabable grabbedObj)
    {
        _currentObject = grabbedObj.GetGameObject();
        grabbedObj.ReleaseObject(grabbedObj.currentPlayer);
        grabbedObj.grabCollider.enabled = false;
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
        Rigidbody rb = _currentObject.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        if (_currentObject.TryGetComponent<IGrabable>(out IGrabable grabObj))
        {
            grabObj.grabCollider.enabled = true;
        }

        GameObject removedObj = _currentObject;
        _currentObject = null;
        return removedObj;
    }

    private void CutIngredient()
    {
        cuttingProgress++;
        RagdollController rc = _currentPlayer.GetComponent<RagdollController>();
        PoseHelper.SetPlayerPose(rc, _postChopPose);

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
        cuttingProgress = 0; // Reset for next ingredient

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
        ExitCutState(_currentPlayer.gameObject);
    }

    private void RaiseHand()
    {
        RagdollController rc = _currentPlayer.GetComponent<RagdollController>();
        PoseHelper.SetPlayerPose(rc, _preChopPose);
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

    private void EnterCutState(GameObject player)
    {
        if (player.TryGetComponent<PlayerInteraction>(out PlayerInteraction pi))
        {
            pi.enabled = false;
        }
        GenericEvent<OnAlternateInteractInput>.GetEvent(player.name).AddListener(ExitCutState);

        Player playerScript = player.GetComponent<Player>();
        _currentPlayer = playerScript;
        playerScript.SetToLockedMode(_playerSnapPoint.position, _playerSnapPoint.rotation, _preChopPose);
        GenericEvent<OnPerformStationActionCancel>.GetEvent(player.name).AddListener(RaiseHand);

        GenericEvent<OnPerformStationAction>.GetEvent(player.name).AddListener(CutIngredient);
        ChangeState(CuttingState.Cutting);

    }

    private void ExitCutState(GameObject player)
    {
        RagdollController rc = player.GetComponent<RagdollController>();
        Transform rootTransform = rc.GetPelvis().gameObject.transform;
        GenericEvent<OnPerformStationActionCancel>.GetEvent(player.name).RemoveListener(RaiseHand);
        GenericEvent<OnPerformStationAction>.GetEvent(player.name).RemoveListener(CutIngredient);
        GenericEvent<OnAlternateInteractInput>.GetEvent(player.name).RemoveListener(ExitCutState);
        // re-enable player interaction
        if (player.TryGetComponent<PlayerInteraction>(out PlayerInteraction pi))
        {
            pi.enabled = true;
        }

        foreach (KeyValuePair<string, RagdollJoint> pair in rc.RagdollDict)
        {
            RagdollJoint joint = pair.Value;
            rc.SetJointToOriginalRot(joint);
        }


        Rigidbody rb = rootTransform.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rc.HardResetPose();
        _currentPlayer.ChangeState(_currentPlayer._defaultStateInstance);
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


}
