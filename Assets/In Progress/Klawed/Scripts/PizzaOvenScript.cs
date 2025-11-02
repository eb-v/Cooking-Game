using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;


public class PizzaOvenScript : MonoBehaviour
{
    [SerializeField] private string _assignedChannel = "DefaultChannel";
    [SerializeField] private HingeJoint _ovenDoorHinge;

    [SerializeField] private float _openAngle = 90f;
    [SerializeField] private float _closeAngle = 0f;

    [SerializeField] private bool _useCustomChannel = false;

    public PizzaDoughBase pizzaInOven;

    [Header("State Machine States")]
    [SerializeField] private BaseStateSO _idleStateBase;
    [SerializeField] private BaseStateSO _cookingStateBase;

    private StateMachine _stateMachine;

    public BaseStateSO _idleStateInstance;
    public BaseStateSO _cookingStateInstance;

    public string currentStateName;
    public bool isClosed = true;

    private void Awake()
    {
        if (_useCustomChannel)
        {
            GenericEvent<InteractEvent>.GetEvent(_assignedChannel).AddListener(OnInteract);
            GenericEvent<PizzaDoughEnteredOvenEvent>.GetEvent(_assignedChannel).AddListener(AddPizza);
            GenericEvent<PizzaDoughExitedOvenEvent>.GetEvent(_assignedChannel).AddListener(RemovePizza);
        }
        else
        {
            GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(OnInteract);
            GenericEvent<PizzaDoughEnteredOvenEvent>.GetEvent(gameObject.name).AddListener(AddPizza);
            GenericEvent<PizzaDoughExitedOvenEvent>.GetEvent(gameObject.name).AddListener(RemovePizza);
        }


        _idleStateInstance = Instantiate(_idleStateBase);
        _cookingStateInstance = Instantiate(_cookingStateBase);

        _stateMachine = new StateMachine();
    }


    private void Start()
    {
        _stateMachine.Initialize(_idleStateInstance);

        _idleStateInstance.Initialize(gameObject, _stateMachine);
        _cookingStateInstance.Initialize(gameObject, _stateMachine);
    }

    private void Update()
    {
        _stateMachine.RunCurrentStateLogic();

        if (_stateMachine.GetCurrentState() == _idleStateInstance)
        {
             currentStateName = "Idle State";
        }
        else if (_stateMachine.GetCurrentState() == _cookingStateInstance)
        {
             currentStateName = "Cooking State";
        }

    }



    private void OnInteract(GameObject player)
    {
        // Close or open pizza oven
        JointSpring spring = _ovenDoorHinge.spring;
        float targetPos = spring.targetPosition;

        if (targetPos == _openAngle)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    public bool IsClosed()
    {
        return isClosed;
    }

    public bool PizzaInside() => pizzaInOven != null;

    

    public void OpenDoor()
    {
        JointSpring spring = _ovenDoorHinge.spring;
        spring.targetPosition = _openAngle;
        _ovenDoorHinge.spring = spring;
        isClosed = false;
    }

    public void CloseDoor()
    {
        JointSpring spring = _ovenDoorHinge.spring;
        spring.targetPosition = _closeAngle;
        _ovenDoorHinge.spring = spring;
        isClosed = true;
    }

    public void RemovePizza(GameObject pizzaObj)
    {
        if (pizzaObj.GetInstanceID() == pizzaInOven.gameObject.GetInstanceID())
        {
            pizzaInOven = null;
        }
    }

    private void AddPizza(GameObject pizzaObj)
    {
        PizzaDoughBase pizza = pizzaObj.GetComponent<PizzaDoughBase>();
        pizzaInOven = pizza;
    }

}
