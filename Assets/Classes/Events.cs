using UnityEngine;
using UnityEngine.Events;

  public class OnMoveInput : UnityEvent<Vector2> { }
  public class OnJumpInput : UnityEvent { }

  public class Move : UnityEvent { }
  public class Idle : UnityEvent { }
  public class HasLanded : UnityEvent { }

  public class GroundedStatusChanged : UnityEvent<bool> { }

