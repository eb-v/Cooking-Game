using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

    #region State Machine Variables

    public ObjectStateMachine stateMachine { get; private set; }
    [SerializeField] private BaseStateSO initialState;
    #endregion

    #region Scriptable Object Variables
    [SerializeField] private BaseStateSO[] availableStates;
    #endregion

    public Dictionary<System.Type, BaseStateSO> stateInstanceMap;


}
