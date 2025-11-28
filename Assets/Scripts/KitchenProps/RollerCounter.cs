using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class RollerCounter : MonoBehaviour
{

    [SerializeField] private Ingredient pizzaDough;

    [SerializeField] private Transform _objectSnapPoint;
    [SerializeField] private Transform _playerSnapPoint;
    [SerializeField] private GameObject _currentObject;
    [SerializeField] private Player _currentPlayer;
    [SerializeField] private RuntimeAnimatorController _rollingAnimator;
    private RollCounterState _currentState = RollCounterState.Idle;
    public bool counterHasDough => _currentObject != null;
    public bool notInUse => _currentPlayer == null;

    private enum RollCounterState
    {
        Idle,
        InUse
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
        GrabScript playerGrabScript = player.GetComponent<GrabScript>();

        if (playerGrabScript.IsGrabbing)
        {
            Grabable grabComponent = playerGrabScript.grabbedObject;

            if (IsItemPizzaDough(grabComponent))
            {
                PlaceDoughOntoCounter(grabComponent);
            }
        }
        else if (counterHasDough && notInUse)
        {
            EnterInUseState(player);
        }
    }

    private void OnAltInteract(GameObject player)
    {
        GrabScript playerGrabScript = player.GetComponent<GrabScript>();
        if (!playerGrabScript.IsGrabbing && counterHasDough)
        {
            Grabable pizzaDough = RemoveDoughFromCounter();
            playerGrabScript.MakePlayerGrabObject(pizzaDough);
        }
    }


    private bool IsItemPizzaDough(Grabable grabbedObject)
    {
        if (grabbedObject.TryGetComponent<IngredientScript>(out IngredientScript ingredient))
        {
            if (ingredient.Ingredient == pizzaDough)
            {
                return true;
            }
        }
        return false;

    }

    private void PlaceDoughOntoCounter(Grabable grabComponent)
    {
        _currentObject = grabComponent.gameObject;
        grabComponent.Release();
        grabComponent.grabCollider.enabled = false;
        _currentObject.transform.position = _objectSnapPoint.position;
        _currentObject.transform.rotation = Quaternion.identity;
        Rigidbody rb = _currentObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private Grabable RemoveDoughFromCounter()
    {
        Rigidbody rb = _currentObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        Grabable grabable = _currentObject.GetComponent<Grabable>();
        grabable.grabCollider.enabled = true;

        Grabable removedGrabable = _currentObject.GetComponent<Grabable>();
        _currentObject = null;

        return removedGrabable;
    }


   

    private void EnterInUseState(GameObject player)
    {
        _currentPlayer = player.GetComponent<Player>();
        _currentPlayer.GetComponent<PlayerInteraction>().enabled = false;
        GenericEvent<OnAlternateInteractInput>.GetEvent(_currentPlayer.name).AddListener(ExitInUseState);
        GenericEvent<OnPerformStationAction>.GetEvent(_currentPlayer.name).AddListener(RollPizzaDough);
        
        Transform rootTransform = _currentPlayer.GetComponent<RagdollController>().GetPelvis().gameObject.transform;
        rootTransform.position = _playerSnapPoint.position;
        rootTransform.rotation = _playerSnapPoint.rotation;

        _currentPlayer.SwitchToAnimationMode(_rollingAnimator);       
        ChangeState(RollCounterState.InUse);

    }

    private void ExitInUseState()
    {
        _currentPlayer.ChangeState(_currentPlayer._defaultStateInstance);

        GenericEvent<OnAlternateInteractInput>.GetEvent(_currentPlayer.name).RemoveListener(ExitInUseState);
        GenericEvent<OnPerformStationAction>.GetEvent(_currentPlayer.name).RemoveListener(RollPizzaDough);
        _currentPlayer.GetComponent<PlayerInteraction>().enabled = true;
        _currentPlayer = null;
        ChangeState(RollCounterState.Idle);
    }

    

    private void RollPizzaDough()
    {
    }

    private void ChangeState(RollCounterState newState)
    {
        _currentState = newState;
    }

}
